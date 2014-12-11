namespace UELib.Decoding
{
    public interface IBufferDecoder
    {
        void PreDecode( IUnrealStream stream );
        void DecodeBuild( IUnrealStream stream, UnrealPackage.GameBuild build );
        int DecodeRead(byte[] array, int offset, int count);
        //Lineage 2 and America's Army have "pre header" infront of the real package
        int PositionOffset { get; }
        //Stream.ReadByte did not call Read(byte[] array, int offset, int count)
        //so we need a separated method
        byte DecodeByte(byte b);
    }
}
