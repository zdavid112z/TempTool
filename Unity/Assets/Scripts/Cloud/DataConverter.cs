using System;

namespace CloudAPI
{
    internal class DataConverter
    {
        public static FileParameterDataBin FromBytes(byte[] data, FileParameterInfo info)
        {
            int count = info.num_dates * info.num_layers * info.width * info.height;
            if (data.Length != sizeof(float) * count)
                return null;

            float[] buffer= new float[count];
            Buffer.BlockCopy(data, 0, buffer, 0, data.Length);
            return new FileParameterDataBin()
            {
                data = buffer,
                info = info
            };
        }
    }

}