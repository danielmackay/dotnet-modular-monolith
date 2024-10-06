namespace Common.SharedKernel.Domain;

public static class Uuid
{
    public static Guid Create() => Guid.CreateVersion7();
}