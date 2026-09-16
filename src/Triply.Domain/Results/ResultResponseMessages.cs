namespace Triply.Domain.Results;

public static class ResultResponseMessages
{
    public static class Success
    {
        public static readonly ResultMessage Created = new("CREATED", "Resource created successfully.");
        public static readonly ResultMessage Ok = new("SUCCESS", "Operation completed successfully.");
        public static readonly ResultMessage NoContent = new("DELETED", "Resource deleted successfully.");
    }
}