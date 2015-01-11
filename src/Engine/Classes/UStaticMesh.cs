using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UELib.Core;
using UELib;
namespace UELib.Engine
{
    [UnrealRegisterClass]
    public class UStaticMesh : UPrimitive, IUnrealViewable
    {
        public UArray<SMeshFace> Faces { get; private set; }
        public UArray<SMeshVertex> Verteces { get; private set; }
        public UArray<UColor> vertex_colors_1 { get; private set; }
        public UArray<UColor> vertex_colors_2 { get; private set; }

        public UArray<SMeshCoords> texture_coords { get; private set; }
        public ushort[] vertex_indicies_1;
        public ushort[] vertex_indicies_2;
        public int NumVerts;

        protected override void Deserialize()
        {
            base.Deserialize();

            Faces = new UArray<SMeshFace>();
            Faces.Deserialize(_Buffer, delegate(SMeshFace mm) { mm.Owner = this; });
            BBox another_bb = new BBox(_Buffer);
            Verteces = new UArray<SMeshVertex>();
            Verteces.Deserialize(_Buffer, delegate(SMeshVertex mm) { mm.Owner = this; });
            Buffer.ReadInt32();
            vertex_colors_1 = new UArray<UColor>();
            vertex_colors_1.Deserialize(_Buffer, delegate(UColor mm) { mm.Owner = this; });
            _Buffer.ReadInt32();
            vertex_colors_2 = new UArray<UColor>();
            vertex_colors_2.Deserialize(_Buffer, delegate(UColor mm) { mm.Owner = this; });
            _Buffer.ReadInt32();
            texture_coords = new UArray<SMeshCoords>();
            texture_coords.Deserialize(_Buffer, delegate(SMeshCoords mm) { mm.Owner = this; });
            int size = _Buffer.ReadIndex();
            vertex_indicies_1 = new ushort[size];
            for (int i = 0; i < size; i++)
            {
                vertex_indicies_1[i] = _Buffer.ReadUInt16();
            }
            _Buffer.ReadInt32();
            size = _Buffer.ReadIndex();
            vertex_indicies_2 = new ushort[size];
            for (int i = 0; i < size; i++)
            {
                vertex_indicies_2[i] = _Buffer.ReadUInt16();
            }
            NumVerts = _Buffer.ReadInt32();
        }
    }
    public class SMeshCoord : IUnrealSerializableClass
    {
        public UV UV;
        public SMeshCoords Owner;
        public void Serialize(IUnrealStream stream)
        {
            throw new NotImplementedException();

        }
        public void Deserialize(IUnrealStream stream)
        {
            UV = new UV(stream.ReadFloat(), stream.ReadFloat());
        }
    }
    public class SMeshCoords : IUnrealSerializableClass
    {
        public UArray<SMeshCoord> elements { get; private set; }
        public UStaticMesh Owner;
        public void Serialize(IUnrealStream stream)
        {
            throw new NotImplementedException();
        }
        public void Deserialize(IUnrealStream stream)
        {
            elements = new UArray<SMeshCoord>();
            elements.Deserialize(stream, delegate(SMeshCoord mm) { mm.Owner = this; });
            stream.ReadInt32();
            stream.ReadInt32();
        }
    }
    public class UColor : IUnrealSerializableClass
    {
        public byte R;
        public byte G;
        public byte B;
        public byte A;
        public UStaticMesh Owner;
        public void Serialize(IUnrealStream stream)
        {
            throw new NotImplementedException();
        }
        public void Deserialize(IUnrealStream stream)
        {
            B = stream.ReadByte();
            G = stream.ReadByte();
            R = stream.ReadByte();
            A = stream.ReadByte();
        }
    }
    public class SMeshVertex : IUnrealSerializableClass
    {
        public UVector Location;
        public UVector Normal;
        public UStaticMesh Owner;
        public void Serialize(IUnrealStream stream)
        {
            throw new NotImplementedException();
        }

        public void Deserialize(IUnrealStream stream)
        {
            Location = new UVector(stream.ReadFloat(), stream.ReadFloat(), stream.ReadFloat());
            Normal = new UVector(stream.ReadFloat(), stream.ReadFloat(), stream.ReadFloat());
        }
    }
    public class SMeshFace : IUnrealSerializableClass
    {
        private int unknown;
        public short index_offset;
        private short unknown01;
        public short vertex_max;
        public short triangle_count;
        public short triangle_max;
        public UStaticMesh Owner;
        public void Serialize(IUnrealStream stream)
        {
            throw new NotImplementedException();
        }

        public void Deserialize(IUnrealStream stream)
        {
            unknown = stream.ReadInt32();
            index_offset = stream.ReadInt16();
            unknown01 = stream.ReadInt16();
            vertex_max = stream.ReadInt16();
            triangle_count = stream.ReadInt16();
            triangle_max = stream.ReadInt16();
        }

    }
    public struct BBox
    {
        public UVector Min;
        public UVector Max;
        public byte is_valid;
        public BBox(UObjectStream _Buffer)
        {
            Min = new UVector(_Buffer);
            Max = new UVector(_Buffer);
            is_valid = _Buffer.ReadByte();
        }
    }
    public struct BSphere
    {
        private UVector m_Sphere;

        public float X
        {
            get
            {
                return m_Sphere.X;
            }
        }
        public float Y
        {
            get
            {
                return m_Sphere.Y;
            }
        }
        public float Z
        {
            get
            {
                return m_Sphere.Z;
            }
        }
        public float Radius;
        public BSphere(UObjectStream _Buffer)
        {
            m_Sphere = new UVector(_Buffer);
            Radius = _Buffer.ReadFloat();
        }

    }
    public class UVector
    {
        public float X;
        public float Y;
        public float Z;
        public UVector(UDefaultProperty property)
        {
            string value = property.Decompile();
            Regex myRegex = new Regex(@"^(?<name>\w+)=.X=(?<X>[0-9]*(?:\.[0-9]*)?),Y=(?<Y>[0-9]*(?:\.[0-9]*)?),Z=(?<Z>[0-9]*(?:\.[0-9]*)?).$", RegexOptions.IgnoreCase);
            Match m = myRegex.Match(value);
            X = Single.Parse(m.Groups["X"].ToString());
            Y = Single.Parse(m.Groups["Y"].ToString());
            Z = Single.Parse(m.Groups["Z"].ToString());
        }
        public UVector()
        {
        }
        public UVector(float x, float y, float z)
        {
            X = x;
            Y = y;
            Z = z;
        }
        public UVector(UObjectStream _Buffer)
        {
            X = _Buffer.ReadFloat();
            Y = _Buffer.ReadFloat();
            Z = _Buffer.ReadFloat();
        }
        public float[] ToArray()
        {
            return new float[] { X, Y, Z };
        }
    }
    public class UV
    {
        public float U;
        public float V;
        public UV(float u, float v)
        {
            U = u;
            V = v;
        }
        public float[] ToArray()
        {
            return new float[] { U, V };
        }
    }
    public class UPrimitive : UObject
    {
        public BBox BoundingBox;
        public BSphere BoundingSphere;
        protected override void Deserialize()
        {
            base.Deserialize();
            BoundingBox = new BBox(this._Buffer);
            BoundingSphere = new BSphere(this._Buffer);
        }
    }
}
