using System.IO;
using UnityEngine;

namespace ShimmerFramework
{
    public class ByteOperation : BaseManager<ByteOperation>
    {
        public byte[] ReadFileToByte(string path)
        {
            FileStream fileStream = new FileStream(path, FileMode.Open, FileAccess.Read);

            fileStream.Seek(0, SeekOrigin.Begin);

            byte[] buffer = new byte[fileStream.Length]; //创建文件长度的buffer  

            fileStream.Read(buffer, 0, (int)fileStream.Length);

            fileStream.Close();

            fileStream.Dispose();

            fileStream = null;

            return buffer;
        }


#if Addressable

#else

        public T WriteFileToByte<T>(string path, byte[] buffer) where T : Object
        {
            //使用using 在资源使用完毕后释放资源
            using (FileStream fileStream = new FileStream(path, FileMode.OpenOrCreate, FileAccess.Write))
            {
                fileStream.Seek(0, SeekOrigin.Begin);

                if (File.Exists(path))
                {
                    fileStream.Write(buffer, 0, (int)buffer.Length);

                }
                else
                {
                    File.Create(path);
                    fileStream.Write(buffer, 0, (int)buffer.Length);
                }

                fileStream.Flush();


                T res = ResourcesManager.GetInstance().LoadAsset<T>(path.Substring(50, path.Length - 54));
                return res;

            }
        }
    }


    //两种方法，这种直接使用File静态类WriteAllBytes方法实现
    //File.WriteAllBytes(Application.persistentDataPath+ "/new.png", FileOperationManager.GetInstance().ReadFileToByte(Application.dataPath + "/UserDate/HEAD.png"));
#endif

 }
