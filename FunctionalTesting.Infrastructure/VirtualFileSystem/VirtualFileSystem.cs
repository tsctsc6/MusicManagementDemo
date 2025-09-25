namespace FunctionalTesting.Infrastructure.VirtualFileSystem;

internal static class VirtualFileSystem
{
    public static VirtualStorage[] VirtualStorages =
    [
        new("X:\\storage1", [new("a.flac", "a", "b", "c")]),
    ];
}
