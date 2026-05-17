using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ObjToFacade
{
    public enum Type
    {
        WIREFRAME = 0,
        FILLED = 2,
        OPAQUE = 1 //Not a real type, it just turns out anything other than 0, and 2 will result in an invisible model.
    }

    public enum TextureID
    {
        FLAT_SHADE = -1
    }

    public enum ColorID
    {
        Beige = 1,
        PastelBlue = 2,
        MediumGray = 3,
        DarkSlateBlue = 4,
        DarkerGray = 5,
        MutedBlue = 6,
        PureWhite = 7,
        DarkEspressoBrown = 8,
        WarmCream = 9,
        GoldenYellow = 10,
        Orange = 11,
        OffWhiteIvory = 12,
        LightGray = 13,
        PaleMintGreen = 14,
        IceBlue = 15,
        PureWhiteAlt1 = 16,
        PureWhiteAlt2 = 17,
        BrightNeonYellow = 18,
        MediumLightGray = 19,
        MidGray50Percent = 20,
        PeriwinkleBlue = 21,
        MutedRosyBrown = 22,
        DeepShadowRoseBrown = 23,
        DarkWalnutBrown = 24,
        WarmSandyBeige = 25,
        MutedSageGreen = 26,
        DarkOliveGreen = 27,
        MutedLightBrown = 28,
        ClassicWoodBrown = 29,
        PaleWarmSkinAccent = 30,
        LightGreen = 31,
        WarmPaleKhaki = 32,
        TranslucentBlueOverlay = 33,
        PaleWarmYellow = 34,
        LightSkinTint = 35,
        TranslucentDarkOverlay = 36,
        PureWhiteAlt3 = 37,
        PureBlackFallback = 38,
        BrightRed = 39,
        BrightBlue = 40,
        BrightGreen = 41,
        MagentaPurple = 42,
        SkyBlue = 43,
        DarkRedBrown = 44,
        MediumRedBrown = 45,
        LightWoodBrown = 46,
        TanSandyBrown = 47,
        VeryDarkWood = 48,
        DarkSepiaBrown = 49,
        MutedBrownShading = 50,
        KhakiBrown = 51,
        NearTotalBlack5Percent = 52,
        VeryDarkCharcoal10Percent = 53,
        DarkCharcoal15Percent = 54,
        DeepCharcoal20Percent = 55,
        DarkGunmetalGray25Percent = 56,
        GunmetalGray30Percent = 57,
        MediumDarkGray35Percent = 58,
        StandardDarkGray40Percent = 59,
        MidDarkGray45Percent = 60
    }

    public static class FmConverter
    {
        public static string ToFacadeModelText(this Obj obj)
        {
            byte[] modelInfo = MakeModelInfo(obj.Faces);
            byte[] modelLines = MakeQuadPolys(obj.Faces, (ColorID)55, 1, Type.FILLED, TextureID.FLAT_SHADE);
            byte[] modelFrame = MakeModelFrame(obj.Vertices);

            StringBuilder sb = new StringBuilder();

            sb.AppendLine("[FACADE_MODEL_START]");
            sb.AppendLine($"Name: {obj.Name ?? "Unnamed"}");
            sb.AppendLine($"VerticesCount: {obj.Vertices.Length / 3}");
            sb.AppendLine($"FacesCount: {obj.Faces.Length}");

            sb.AppendLine("// --- MODEL INFO HEX ---");
            sb.AppendLine(BitConverter.ToString(modelInfo).Replace("-", ""));

            sb.AppendLine("// --- MODEL LINES HEX ---");
            sb.AppendLine(BitConverter.ToString(modelLines).Replace("-", ""));

            sb.AppendLine("// --- MODEL FRAME HEX ---");
            sb.AppendLine(BitConverter.ToString(modelFrame).Replace("-", ""));
            sb.AppendLine("[FACADE_MODEL_END]");

            return sb.ToString();
        }

        private static byte[] MakeQuadPolys(Face[] faces, ColorID faceColor, int lineWidth, Type type = Type.FILLED, TextureID textureId = TextureID.FLAT_SHADE)
        {
            using (MemoryStream ms = new MemoryStream())
            using (BinaryWriter writer = new BinaryWriter(ms))
            {
                foreach (var face in faces)
                {
                    // 15 ints per quad
                    writer.Write(face.A);       // v0
                    writer.Write(face.B);       // v1
                    writer.Write(face.C);       // v2
                    writer.Write(face.C);       // v3 (degenerate for tris)

                    writer.Write((int)type);    // type
                    writer.Write((int)faceColor);// fill

                    writer.Write(0);            // ecol0
                    writer.Write(0);            // ecol1
                    writer.Write(0);            // ecol2
                    writer.Write(0);            // ecol3

                    writer.Write(12);           // ew0
                    writer.Write(12);           // ew1
                    writer.Write(12);           // ew2
                    writer.Write(12);           // ew3

                    writer.Write((int)textureId); // texture
                }

                // Sentinel: 14x 0x9A020000 - no fucking clue what this is though
                for (int i = 0; i < 14; i++)
                    writer.Write(0x0000029a);

                return ms.ToArray();
            }
        }

        private static byte[] MakeModelInfo(Face[] faces)
        {
            using (MemoryStream ms = new MemoryStream())
            using (BinaryWriter writer = new BinaryWriter(ms))
            {
                foreach (var face in faces)
                {
                    writer.Write(2.0f);
                    writer.Write(1.0f);
                    writer.Write(-1.0f);
                    writer.Write(-1.0f);
                    writer.Write(0.0f);
                    writer.Write(4.0f);
                    writer.Write(1.0f);
                }

                //Each info ends with a "666.0f" sentinel line of bytes
                for (int i = 0; i < 6; i++)
                    writer.Write(666.0f);

                return ms.ToArray();
            }
        }

        private static byte[] MakeModelFrame(float[] vertices, float scaleFactor = 50.0f)
        {
            using (MemoryStream ms = new MemoryStream())
            using (BinaryWriter writer = new BinaryWriter(ms))
            {
                for (int i = 0; i < vertices.Length; i += 3)
                {
                    writer.Write(vertices[i] * scaleFactor); // X
                    writer.Write(vertices[i + 1] * scaleFactor); // Y
                    writer.Write(vertices[i + 2] * scaleFactor); // Z
                }

                // Same here but the sentinel is about 4x 666.0f
                for (int i = 0; i < 4; i++)
                    writer.Write(666.0f);

                return ms.ToArray();
            }
        }
    }
}