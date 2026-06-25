using ArasTrader.Application.Interfaces.OrderProcessing;
using ArasTrader.Domain.Enums;
using ArasTrader.Infrastructure.Options;
using ArasTrader.Infrastructure.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Npgsql;

namespace ArasTrader.Infrastructure.Persistence.Repositories.OrderProcessing;

internal class PostgresOrderClaimService : IOrderClaimService
{
    private const string PendingStatus = "pendingStatus";
    private const string InProgressStatus = "inProgressStatus";
    private const string BatchSize = "batchSize";
    private const string ProcessingTimeout = "processingTimeout";

    private readonly ArasTraderDbContext _dbContext;
    private readonly OrderProcessingOptions _orderProcessingOptions;

    public PostgresOrderClaimService(ArasTraderDbContext dbContext, IOptions<OrderProcessingOptions> orderProcessingOptions)
    {
        _dbContext = dbContext;
        _orderProcessingOptions = orderProcessingOptions.Value;
    }

    public async Task<IReadOnlyList<int>> ClaimPendingOrdersAsync(CancellationToken cancellationToken)
    {
        const string sql = """
                WITH claimed_orders AS
                (
                    SELECT "Id"
                    FROM public."Orders"
                    WHERE
                    (
                        "Status" = @pendingStatus
                    )
                    OR
                    (
                        "Status" = @inProgressStatus
                        AND "ProcessingStartedAt" <= NOW() - @processingTimeout
                    )
                    ORDER BY "CreatedAt"
                    LIMIT @batchSize
                    FOR UPDATE SKIP LOCKED
                )
                UPDATE public."Orders" o
                SET
                    "Status" = @inProgressStatus,
                    "ProcessingStartedAt" = NOW(),
                    "ModifiedAt" = NOW()
                FROM claimed_orders c
                WHERE o."Id" = c."Id"
                RETURNING o."Id";
                """;

        var connection = (NpgsqlConnection)_dbContext.Database.GetDbConnection();

        if (connection.State != System.Data.ConnectionState.Open)
            await connection.OpenAsync(cancellationToken);

        await using var command = connection.CreateCommand();

        command.CommandText = sql;
        command.Parameters.AddWithValue(PendingStatus, (int)OrderStatus.Pending);
        command.Parameters.AddWithValue(InProgressStatus, (int)OrderStatus.InProgress);
        command.Parameters.AddWithValue(BatchSize, _orderProcessingOptions.BatchSize);
        command.Parameters.AddWithValue(ProcessingTimeout, _orderProcessingOptions.ProcessingTimeout);

        var result = new List<int>();

        await using var reader =
            await command.ExecuteReaderAsync(cancellationToken);

        while (await reader.ReadAsync(cancellationToken))
        {
            result.Add(reader.GetInt32(0));
        }

        return result;
    }
}
