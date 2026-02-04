using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace BlogWebApp.Validators
{
    public static class DbUpdateExceptionExtension
    {
        public static bool IsUniqueViolation(this DbUpdateException ex)
        {
            if (ex.InnerException is PostgresException pgEx)
            {
                return pgEx.SqlState == PostgresErrorCodes.UniqueViolation;
            }

            return false;
        }
    }
}
