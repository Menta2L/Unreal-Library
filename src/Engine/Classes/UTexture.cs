using System;
using UELib.Core;
using UELib;

namespace UELib.Engine
{
    public enum ETextureFormat
    {
        TEXF_P8,			// used 8-bit palette
        TEXF_RGBA7,
        TEXF_RGB16,			// 16-bit texture
        TEXF_DXT1,
        TEXF_RGB8,
        TEXF_RGBA8,			// 32-bit texture
        TEXF_NODATA,
        TEXF_DXT3,
        TEXF_DXT5,
        TEXF_L8,			// 8-bit grayscale
        TEXF_G16,			// 16-bit grayscale (terrain heightmaps)
        TEXF_RRRGGGBBB,
        // Tribes texture formats
        TEXF_CxV8U8,
        TEXF_DXT5N,			// Note: in Bioshock this value has name 3DC, but really DXT5N is used
        TEXF_3DC,			// BC5 compression
    };
    [UnrealRegisterClass]
    public class UTexture : UBitmapMaterial, IUnrealViewable
    {
        public UArray<MipMap> MipMaps{ get; private set; }

        public UTexture()
        {
            ShouldDeserializeOnDemand = true;
        }

        protected override void Deserialize()
        {
            base.Deserialize();
            MipMaps = new UArray<MipMap>();
            MipMaps.Deserialize( _Buffer, delegate( MipMap mm ){ mm.Owner = this; } );
        }

        public class MipMap : IUnrealSerializableClass
        {
            public enum CompressionFormat
            {
                RGBA8,
            };

            public UTexture Owner;

            public uint WidthOffset;
            public byte[] Pixels;
            public uint Width;
            public uint Height;
            public byte BitsWidth;
            public byte BitsHeight;

            public void Serialize( IUnrealStream stream )
            {
                throw new NotImplementedException();
            }

            public void Deserialize( IUnrealStream stream )
            {
                
                stream.ReadUInt32();
                int mipMapSize = stream.ReadIndex();
                Pixels = new byte[mipMapSize];
                stream.Read(Pixels, 0, mipMapSize);
                Width = stream.ReadUInt32();
                Height = stream.ReadUInt32();
                BitsWidth = stream.ReadByte();
                BitsHeight = stream.ReadByte();
            }
        }
    }
}