using System;
using UELib.Core;
using UELib;
using System.Text;
using System.Reflection;
using System.IO;
namespace UELib.Engine
{

/*
UE2 MATERIALS TREE:
~~~~~~~~~~~~~~~~~~~
-	Material
-		Combiner
-		Modifier
			ColorModifier
-			FinalBlend
			MaterialSequence
			MaterialSwitch
			OpacityModifier
-			TexModifier
				TexCoordSource
-				TexEnvMap
				TexMatrix
-				TexOscillator
					TexOscillatorTriggered
-				TexPanner
					TexPannerTriggered
-				TexRotator
-				TexScaler
				VariableTexPanner
-		RenderedMaterial
-			BitmapMaterial
				ScriptedTexture
				ShadowBitmapMaterial
-				Texture
					Cubemap
-			ConstantMaterial
-				ConstantColor
				FadeColor
			ParticleMaterial
			ProjectorMaterial
-			Shader
			TerrainMaterial
			VertexColor
*/
    public class UMaterial : UObject 
    {
        public FLineageShaderProperty ShaderProp;
        public string ShaderCode;
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
                    _Buffer.ReadInt32();
                }
                if (Package.Version >= 123 && Package.LicenseeVersion >= 30 && Package.LicenseeVersion < 37)
                {
                    byte MaterialInfo, TextureTranform, MAX_SAMPLER_NUM, MAX_TEXMAT_NUM, MAX_PASS_NUM, TwoPassRenderState, AlphaRef;
                    if (Package.LicenseeVersion >= 33 && Package.LicenseeVersion < 36)
                    {
                        MaterialInfo = _Buffer.ReadByte();
                    }
                    TextureTranform = _Buffer.ReadByte();
                    MAX_SAMPLER_NUM = _Buffer.ReadByte();
                    MAX_TEXMAT_NUM = _Buffer.ReadByte();
                    MAX_PASS_NUM = _Buffer.ReadByte();
                    TwoPassRenderState = _Buffer.ReadByte();
                    AlphaRef = _Buffer.ReadByte();
                    int SrcBlend, DestBlend, OverriddenFogColor;
                    SrcBlend = _Buffer.ReadInt32();
                    DestBlend = _Buffer.ReadInt32();
                    OverriddenFogColor = _Buffer.ReadInt32();
                    for (int i = 0; i < 8; i++)
                    {
                        byte b1, b2;
                        b1 = _Buffer.ReadByte();
                        if (Package.LicenseeVersion < 36) b2 = _Buffer.ReadByte(); ;
                        for (int j = 0; j < 126; j++)
                        {
                            byte b3;
                            b3 = _Buffer.ReadByte(); 
                        }
                    }
                    // another nested function - serialize FC_* variables
				    byte[] c =new byte[8];
                    c[2] = _Buffer.ReadByte();
                    c[1] = _Buffer.ReadByte();
                    c[0] = _Buffer.ReadByte();
                    c[3] = _Buffer.ReadByte();
                    c[6] = _Buffer.ReadByte();
                    c[5] = _Buffer.ReadByte();
                    c[4] = _Buffer.ReadByte();
                    c[7] = _Buffer.ReadByte();

                     int FC_FadePeriod, FC_FadePhase, FC_ColorFadeType;
                     FC_FadePeriod = _Buffer.ReadInt32();
                     FC_FadePhase = _Buffer.ReadInt32();
                     FC_ColorFadeType = _Buffer.ReadInt32();
                     for (int i = 0; i < 16; i++)
                     {
                         string strTex;
                         strTex = _Buffer.ReadText();
                     }
                     string ShaderCode;
                     ShaderCode = _Buffer.ReadText();
                }
                if (Package.Version >= 123 && Package.LicenseeVersion >= 37)
                {
                    // ShaderProperty + ShaderCode
                     ShaderProp = new FLineageShaderProperty(_Buffer);
                     ShaderCode = _Buffer.ReadText();
                 }
                if (Package.Version >= 123 && Package.LicenseeVersion >= 31)
                {
                    short ver1, ver2;			// 'int MaterialCodeVersion' serialized as 2 words
                    ver1 = _Buffer.ReadInt16();
                    ver2 = _Buffer.ReadInt16();
                }
             }
        }
    }
    public class URenderedMaterial : UMaterial
    {
    }
    public class UBitmapMaterial : URenderedMaterial
    {
        public byte UBits { get; set; }
        public byte VBits { get; set; }	// texture size log2 (number of bits in size value)
        public int USize { get; set; }
        public int VSize { get; set; }
        public int UClamp { get; set; }
        public int VClamp { get; set; }
        public ETextureFormat Format;
        protected override void Deserialize()
        {
            base.Deserialize();
            long pos = _Buffer.Position;
            foreach (UDefaultProperty property in Properties)
            {
                string propertyVal = property.Decompile();

                var kv = propertyVal.Split(new char[] { '=' }, StringSplitOptions.RemoveEmptyEntries);
                if (property.Name == "Format")
                {
                    Format = (ETextureFormat)Convert.ToInt32(kv[1]);
                }
                else
                {
                    SetValue(kv[0], kv[1]);
                }
            }
            _Buffer.Seek(pos, SeekOrigin.Begin);

        }
        // probably not a bad idea to make extension to UObject
        public void SetValue(string propertyName, object propertyVal)
        {
            Type type = this.GetType();

            PropertyInfo propertyInfo = type.GetProperty(propertyName);
            if (propertyInfo == null)
            {
                return;
            }
            Type propertyType = propertyInfo.PropertyType;
            var targetType = IsNullableType(propertyInfo.PropertyType) ? Nullable.GetUnderlyingType(propertyInfo.PropertyType) : propertyInfo.PropertyType;
            propertyVal = Convert.ChangeType(propertyVal, targetType);
            propertyInfo.SetValue(this, propertyVal, null);

        }
        private static bool IsNullableType(Type type)
        {
            return type.IsGenericType && type.GetGenericTypeDefinition().Equals(typeof(Nullable<>));
        }
    }
}
