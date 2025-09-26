namespace FunctionalTesting.Infrastructure.VirtualFileSystem;

internal static class VirtualFileSystem
{
    public static VirtualStorage[] VirtualStorages =
    [
        new(
            "X:\\storage1",
            [
                new(
                    "Right Here Waiting.flac",
                    "Right Here Waiting",
                    "Richard Marx",
                    "情歌精选 CD1"
                ),
                new(
                    "As Long As You Love Me.flac",
                    "As Long As You Love Me",
                    "Backstreet Boys",
                    "Backstreet's Back"
                ),
                new(
                    "Nothings Gonna Change My Love For You.flac",
                    "Nothings Gonna Change My Love For You",
                    "Westlife",
                    "The Love Album Bonus CD"
                ),
                new(
                    "All Out Of Love.flac",
                    "All Out Of Love",
                    "Westlife; Delta Goodrem",
                    "The Love Album"
                ),
                new(
                    "Take Me To Your Heart.flac",
                    "Take Me To Your Heart",
                    "Michael Learns To Rock",
                    "情歌传奇 CD1"
                ),
                new("Home.flac", "Home", "Westlife", "Greatest Hits CD1"),
            ]
        ),
    ];
}
