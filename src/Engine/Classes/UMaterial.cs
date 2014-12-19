using System;
using UELib.Core;
using UELib;
using System.Text;
namespace UELib.Engine
{
    public class UMaterial : UObject 
    {
        public class FLineageMaterialStageProperty : IUnrealSerializableClass
        {
            public string unk1;
            public void Deserialize(IUnrealStream stream)
            {
                unk1=stream.ReadText();
                int c = stream.ReadIndex();
                for (int i = 0; i < c; ++i)
                {
                    string s =stream.ReadText();
                }
            }
            public void Serialize(IUnrealStream stream)
            {
                throw new NotImplementedException();
            }
        }
        public class FLineageShaderProperty {
            // possibly, MaterialInfo, TextureTranform, TwoPassRenderState, AlphaRef
	        public byte			b1, b2;
	        public byte[]		b3 = new byte[5];
            public byte[]       b4 = new byte[5];
            public byte[]       b5 = new byte[5];
            public byte[]       b6 = new byte[5];
            public byte[]       b7 = new byte[5];
            public byte[]       b8 = new byte[5];
	        // possibly, SrcBlend, DestBlend, OverriddenFogColor
	        public int[]				i1= new int[5];
            public int[]				i2= new int[5];
            public int[]				i3= new int[5];
	        // nested structure
	        // possibly, int FC_Color1, FC_Color2 (strange byte order)
	        public byte[]			be= new byte[8];
	        // possibly, float FC_FadePeriod, FC_FadePhase, FC_ColorFadeType
            public int[] ie = new int[3];
            //TArray<FLineageMaterialStageProperty> Stages;
            public UArray<FLineageMaterialStageProperty> Stages { get; private set; }
            public FLineageShaderProperty(UObjectStream stream) {
                b1 = stream.ReadByte();
                b2 = stream.ReadByte();

                if (stream.Package.Version< 129)
                {
                    b3[0] = stream.ReadByte();
                    b4[0] = stream.ReadByte();
                    i1[0] = stream.ReadInt32();
                    i2[0] = stream.ReadInt32();
                    i3[0] = stream.ReadInt32();
                }
                else if (stream.Package.Version == 129)
                {
                    for (int i = 0; i < 5; i++)
                    {
                        b3[i] = stream.ReadByte();
                        b4[i] = stream.ReadByte();
                        i1[i] = stream.ReadInt32();
                        i2[i] = stream.ReadInt32();
                        i3[i] = stream.ReadInt32();
                    }
                }
                else // if (Ar.ArVer >= 130)
                {
                    for (int i = 0; i < 5; i++)
                    {
                        b3[i] = stream.ReadByte();
                        b4[i] = stream.ReadByte();
                        b5[i] = stream.ReadByte();
                        b6[i] = stream.ReadByte();
                        b7[i] = stream.ReadByte();
                        b8[i] = stream.ReadByte(); 
                        i2[i] = stream.ReadInt32();
                        i3[i] = stream.ReadInt32();
                    }
                }
                stream.Read(be, 0, 8);
                //for (int i = 0; i < 8; i++) be[i] = stream.ReadByte();
                for (int i = 0; i < 3; i++) ie[i] = stream.ReadInt32();
                Stages = new UArray<FLineageMaterialStageProperty>();
                Stages.Deserialize(stream);
            }
        }
        protected override void Deserialize()
        {
            base.Deserialize();
            if (Package.Build == UnrealPackage.GameBuild.BuildName.Lineage2) {
                if (Package.Version >= 123 && Package.LicenseeVersion >= 16 && Package.LicenseeVersion < 37) {
                    Buffer.ReadInt32();
                }
                if (Package.Version >= 123 && Package.LicenseeVersion >= 30 && Package.LicenseeVersion < 37)
                {
                    byte MaterialInfo, TextureTranform, MAX_SAMPLER_NUM, MAX_TEXMAT_NUM, MAX_PASS_NUM, TwoPassRenderState, AlphaRef;
                    if (Package.LicenseeVersion >= 33 && Package.LicenseeVersion < 36)
                    {
                        MaterialInfo = Buffer.ReadByte();
                    }
                    TextureTranform = Buffer.ReadByte();
                    MAX_SAMPLER_NUM = Buffer.ReadByte();
                    MAX_TEXMAT_NUM = Buffer.ReadByte();
                    MAX_PASS_NUM = Buffer.ReadByte();
                    TwoPassRenderState = Buffer.ReadByte();
                    AlphaRef = Buffer.ReadByte();
                    int SrcBlend, DestBlend, OverriddenFogColor;
                    SrcBlend = Buffer.ReadInt32();
                    DestBlend = Buffer.ReadInt32();
                    OverriddenFogColor = Buffer.ReadInt32();
                    for (int i = 0; i < 8; i++)
                    {
                        byte b1, b2;
                        b1 = Buffer.ReadByte();
                        if (Package.LicenseeVersion < 36) b2 = Buffer.ReadByte(); ;
                        for (int j = 0; j < 126; j++)
                        {
                            // really, 1Kb of floats and ints ...
                            byte b3;
                            b3 = Buffer.ReadByte(); 
                        }
                    }
                    // another nested function - serialize FC_* variables
				    byte[] c =new byte[8];					// union with "int FC_Color1, FC_Color2" (strange byte order)
				     c[2] =Buffer.ReadByte();
                     c[1] = Buffer.ReadByte();
                     c[0] = Buffer.ReadByte();
                     c[3] = Buffer.ReadByte();
                     c[6] = Buffer.ReadByte();
                     c[5] = Buffer.ReadByte();
                     c[4] = Buffer.ReadByte();
                     c[7] = Buffer.ReadByte();

                     int FC_FadePeriod, FC_FadePhase, FC_ColorFadeType;	// really, floats?
                     FC_FadePeriod = Buffer.ReadInt32();
                     FC_FadePhase = Buffer.ReadInt32();
                     FC_ColorFadeType = Buffer.ReadInt32();
                     for (int i = 0; i < 16; i++)
                     {
                         string strTex;			// strTex[16]
                         strTex = Buffer.ReadText();
                         Console.WriteLine(strTex);
                     }
                     string ShaderCode;
                     ShaderCode = Buffer.ReadText();
                     Console.WriteLine(ShaderCode);
                }
                if (Package.Version >= 123 && Package.LicenseeVersion >= 37)
                {
                    // ShaderProperty + ShaderCode
                    FLineageShaderProperty ShaderProp;
                     string ShaderCode;
                     ShaderProp= new FLineageShaderProperty(Buffer);
                     ShaderCode = Buffer.ReadText();
                 }
                if (Package.Version >= 123 && Package.LicenseeVersion >= 31)
                {
                    short ver1, ver2;			// 'int MaterialCodeVersion' serialized as 2 words
                    ver1= Buffer.ReadInt16();
                    ver2= Buffer.ReadInt16();
                }
             }
        }
    }
}
