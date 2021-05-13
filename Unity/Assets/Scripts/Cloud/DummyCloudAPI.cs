using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using UnityEngine;

namespace CloudAPI
{
    public class DummyCloudAPI : ICloudAPI
    {
        public DummyCloudAPI()
        {
            admins = new List<AdminData>
            {
                new AdminData() { email = "admin1@gmail.com" },
                new AdminData() { email = "admin2@gmail.com" },
                new AdminData() { email = "admin3@gmail.com" },
                new AdminData() { email = "admin4@gmail.com" },
                new AdminData() { email = "bestadmin@gmail.com" },
                new AdminData() { email = "master@temptool.org" },
                new AdminData() { email = "myadmin@temptool.org" },
            };

            files = new List<FileInfoDetailed>
            {
                new FileInfoDetailed()
                {
                    id = "DOGECOIN",
                    filename = "file1.nc",
                    is_permanent = false,
                    last_used_date = 1620754142,
                    size = 1024 * 1024 * 32,
                    uploaded_by = "Anonymous",
                    upload_date = 1620656942,
                    parameters = new FileParameterInfo[]
                    {
                        new FileParameterInfo()
                        {
                            name = "Temperature",
                            description = "Temperature at ground level",
                            element_bytes = 4,
                            num_layers = 1,
                            start_date = 1589034542,
                            end_date = 1589293742,
                            type = "Geo2D",
                            width = 16,
                            height = 16,
                            num_dates = 4,
                            unit = "C",
                            layer_min = 0,
                            layer_max = 0,
                            lat_min = 43.760432f,
                            lon_min = 21.150052f,
                            lat_max = 47.864813f,
                            lon_max = 28.968124f
                        },
                        new FileParameterInfo()
                        {
                            name = "Humidity",
                            description = "Humidity at ground level",
                            element_bytes = 4,
                            num_layers = 1,
                            start_date = 1589034542,
                            end_date = 1589293742,
                            type = "Geo2D",
                            width = 16,
                            height = 16,
                            num_dates = 4,
                            unit = "%",
                            layer_min = 0,
                            layer_max = 0,
                            lat_min = 43.760432f,
                            lon_min = 21.150052f,
                            lat_max = 47.864813f,
                            lon_max = 28.968124f
                        },
                        new FileParameterInfo()
                        {
                            name = "Air Pressure",
                            description = "Air pressure at ground level",
                            element_bytes = 4,
                            num_layers = 1,
                            start_date = 1589034542,
                            end_date = 1589293742,
                            type = "Geo2D",
                            width = 72,
                            height = 48,
                            num_dates = 8,
                            unit = "%",
                            layer_min = 0,
                            layer_max = 0,
                            lat_min = -180,
                            lon_min = 0,
                            lat_max = 180,
                            lon_max = 355
                        }
                    }
                },
                new FileInfoDetailed()
                {
                    id = "SHIBAINUCOIN",
                    filename = "file2.nc",
                    is_permanent = true,
                    last_used_date = 1620570542,
                    size = 1024 * 1024 * 16,
                    uploaded_by = "master@temptool.org",
                    upload_date = 1620397742,
                    parameters = new FileParameterInfo[]
                    {
                        new FileParameterInfo()
                        {
                            name = "t",
                            description = "Temperature",
                            element_bytes = 4,
                            num_layers = 1,
                            start_date = 1559917742,
                            end_date = 1560176942,
                            type = "Geo2D",
                            width = 16,
                            height = 16,
                            num_dates = 4,
                            unit = "C",
                            layer_min = 0,
                            layer_max = 0,
                            lat_min = -2.857927f,
                            lon_min = -65.872914f,
                            lat_max = 7.322292f,
                            lon_max = -81.978124f
                        }
                    }
                },
                new FileInfoDetailed()
                {
                    id = "ara_ara",
                    filename = "usa.nc",
                    is_permanent = true,
                    last_used_date = 1620570542,
                    size = 1024 * 1024 * 16,
                    uploaded_by = "master@temptool.org",
                    upload_date = 1620397742,
                    parameters = new FileParameterInfo[]
                    {
                        new FileParameterInfo()
                        {
                            name = "air_quality",
                            description = "Air Quality",
                            element_bytes = 4,
                            num_layers = 1,
                            start_date = 1559917742,
                            end_date = 1560176942,
                            type = "Geo2D",
                            width = 256,
                            height = 256,
                            num_dates = 2,
                            unit = "C",
                            layer_min = 0,
                            layer_max = 0,
                            lat_min = 22.526144f,
                            lon_min = -121.945710f,
                            lat_max = 44.756328f,
                            lon_max = -68.540308f
                        }
                    }
                }
            };
        }

        public IEnumerator DeleteAdmin(AdminData admin, Action<long> onSuccess, Action<ErrorDetails> onError)
        {
            yield return new WaitForSeconds(waitDelay);

            if (forcedError != null)
            {
                onError(forcedError);
                yield break;
            }

            int count = admins.RemoveAll((AdminData a) => { return a.email == admin.email; });

            if (count != 0)
                onSuccess(200);
            else
                onError(GenProtocolErrorDetails(400));
        }

        public IEnumerator DeleteFile(string fileId, Action<long> onSuccess, Action<ErrorDetails> onError)
        {
            yield return new WaitForSeconds(waitDelay);

            if (forcedError != null)
            {
                onError(forcedError);
                yield break;
            }

            int count = files.RemoveAll((FileInfoDetailed f) => { return f.id == fileId; });

            if (count != 0)
                onSuccess(200);
            else
                onError(GenProtocolErrorDetails(404));
        }

        public IEnumerator GetAdmins(Action<AdminData[], long> onSuccess, Action<ErrorDetails> onError)
        {
            yield return new WaitForSeconds(waitDelay);

            if (forcedError != null)
            {
                onError(forcedError);
                yield break;
            }

            onSuccess(admins.ToArray(), 200);
        }

        public IEnumerator GetFileDetailed(string fileId, Action<FileInfoDetailed, long> onSuccess, Action<ErrorDetails> onError)
        {
            yield return new WaitForSeconds(waitDelay);

            if (forcedError != null)
            {
                onError(forcedError);
                yield break;
            }

            var info = files.Find((FileInfoDetailed f) => { return f.id == fileId; });

            if (info != null)
                onSuccess(info, 200);
            else
                onError(GenProtocolErrorDetails(404));
        }

        public IEnumerator GetFileOriginal(string fileId, Action<byte[], long> onSuccess, Action<ErrorDetails> onError)
        {
            yield return new WaitForSeconds(waitDelay);

            if (forcedError != null)
            {
                onError(forcedError);
                yield break;
            }

            var info = files.Find((FileInfoDetailed f) => { return f.id == fileId; });

            if (info != null)
                onSuccess(Encoding.UTF8.GetBytes($"This is a dummy file with id {fileId}"), 200);
            else
                onError(GenProtocolErrorDetails(404));
        }

        public IEnumerator GetFileParameter(string fileId, FileParameterInfo parameterData, Action<FileParameterDataBin, long> onSuccess, Action<ErrorDetails> onError)
        {
            yield return new WaitForSeconds(waitDelay);

            if (forcedError != null)
            {
                onError(forcedError);
                yield break;
            }

            var info = files.Find((FileInfoDetailed f) => { return f.id == fileId; });

            if (info != null)
            {
                var param = info.parameters.ToList().Find((FileParameterInfo p) => { return p.name == parameterData.name; });
                if (param != null)
                {
                    onSuccess(GenerateRandomMatrix(parameterData), 200);
                    yield break;
                }
            }
            onError(GenProtocolErrorDetails(404));
        }

        public IEnumerator GetFiles(Action<FileInfo[], long> onSuccess, Action<ErrorDetails> onError)
        {
            yield return new WaitForSeconds(waitDelay);

            if (forcedError != null)
            {
                onError(forcedError);
                yield break;
            }

            var result = files.Select(x => new FileInfo()
            {
                id = x.id,
                is_permanent = x.is_permanent,
                last_used_date = x.last_used_date,
                name = x.filename,
                size = x.size,
                uploaded_by = x.uploaded_by,
                upload_date = x.upload_date
            });
            onSuccess(result.ToArray(), 200);
        }

        public IEnumerator PostAdmin(AdminData admin, Action<long> onSuccess, Action<ErrorDetails> onError)
        {
            yield return new WaitForSeconds(waitDelay);

            if (forcedError != null)
            {
                onError(forcedError);
                yield break;
            }

            admins.Add(admin);
            onSuccess(200);
        }

        public IEnumerator PostFile(string filePath, Action<long> onSuccess, Action<ErrorDetails> onError)
        {
            yield return new WaitForSeconds(waitDelay);

            if (forcedError != null)
            {
                onError(forcedError);
                yield break;
            }

            files.Add(new FileInfoDetailed()
            {
                filename = filePath,
                id = $"random id {filePath}"
            });
            onSuccess(200);
        }

        public IEnumerator PostLogin(LoginRequestData request, Action<LoginResponse, long> onSuccess, Action<ErrorDetails> onError)
        {
            yield return new WaitForSeconds(waitDelay);

            if (forcedError != null)
            {
                onError(forcedError);
                yield break;
            }

            onError(GenProtocolErrorDetails(400));
        }

        private FileParameterDataBin GenerateRandomMatrix(FileParameterInfo param)
        {
            var random = new System.Random(param.GetHashCode());
            int count = param.num_dates * param.num_layers * param.height * param.width;
            float[] result = new float[count];
            for (int i = 0; i < count; i++)
                result[i] = (float) random.NextDouble();

            return new FileParameterDataBin()
            {
                data = result,
                info = param
            };
        }

        private ErrorDetails GenProtocolErrorDetails(long code)
        {
            return new ErrorDetails()
            {
                body = null,
                error = WebRequests.GetStatusDescription((int)code),
                responseCode = code,
                result = UnityEngine.Networking.UnityWebRequest.Result.ProtocolError
            };
        }

        public List<AdminData> admins;
        public List<FileInfoDetailed> files;
        public ErrorDetails forcedError = null;
        public float waitDelay = 0.1f;
    }
}
