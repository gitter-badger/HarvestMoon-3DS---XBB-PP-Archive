namespace HM3DSGCR.Archive
{
    public struct XbbFileEntry
    {
        internal long FileOffset;
        internal long FileSize;
        internal long FileNameOffset;
        internal long FileCheckSum;
    }

    public struct PapaFileEntry
    {
        internal long FileOffset;
        internal long FileSize;
    }

    public struct CheckSumEntrySort
    {
        internal long FileCheckSum;
        internal long FileID;
    }
}
