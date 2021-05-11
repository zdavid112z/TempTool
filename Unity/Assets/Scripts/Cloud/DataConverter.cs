using System;

namespace CloudAPI
{
    internal class DataConverter
    {
        public static float[,,,] FromBytes(byte[] data, int dim1, int dim2, int dim3, int dim4)
        {
            if (data.Length != sizeof(float) * dim1 * dim2 * dim3 * dim4)
            {
                return null;
            }

            float[,,,] result = new float[dim1, dim2, dim3, dim4];
            Buffer.BlockCopy(data, 0, result, 0, data.Length);
            return result;
        }
    }

}