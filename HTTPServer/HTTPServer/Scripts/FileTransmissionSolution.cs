using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
namespace Takamachi660.FileTransmissionSolution
{//Version 0.6
    #region CRC32算法
    /// <summary>
    /// CRC32快速检测算法
    /// 
    /// 有稍作修改
    /// </summary>
    public static class CRC32
    {
        static UInt32[] crcTable = {   
         0x0, 0x77073096, 0xee0e612c, 0x990951ba, 0x76dc419, 0x706af48f, 0xe963a535, 0x9e6495a3,   
         0xedb8832, 0x79dcb8a4, 0xe0d5e91e, 0x97d2d988, 0x9b64c2b, 0x7eb17cbd, 0xe7b82d07, 0x90bf1d91,   
         0x1db71064, 0x6ab020f2, 0xf3b97148, 0x84be41de, 0x1adad47d, 0x6ddde4eb, 0xf4d4b551, 0x83d385c7,   
         0x136c9856, 0x646ba8c0, 0xfd62f97a, 0x8a65c9ec, 0x14015c4f, 0x63066cd9, 0xfa0f3d63, 0x8d080df5,   
         0x3b6e20c8, 0x4c69105e, 0xd56041e4, 0xa2677172, 0x3c03e4d1, 0x4b04d447, 0xd20d85fd, 0xa50ab56b,   
         0x35b5a8fa, 0x42b2986c, 0xdbbbc9d6, 0xacbcf940, 0x32d86ce3, 0x45df5c75, 0xdcd60dcf, 0xabd13d59,   
         0x26d930ac, 0x51de003a, 0xc8d75180, 0xbfd06116, 0x21b4f4b5, 0x56b3c423, 0xcfba9599, 0xb8bda50f,   
         0x2802b89e, 0x5f058808, 0xc60cd9b2, 0xb10be924, 0x2f6f7c87, 0x58684c11, 0xc1611dab, 0xb6662d3d,   
         0x76dc4190, 0x1db7106, 0x98d220bc, 0xefd5102a, 0x71b18589, 0x6b6b51f, 0x9fbfe4a5, 0xe8b8d433,   
         0x7807c9a2, 0xf00f934, 0x9609a88e, 0xe10e9818, 0x7f6a0dbb, 0x86d3d2d, 0x91646c97, 0xe6635c01,   
         0x6b6b51f4, 0x1c6c6162, 0x856530d8, 0xf262004e, 0x6c0695ed, 0x1b01a57b, 0x8208f4c1, 0xf50fc457,   
         0x65b0d9c6, 0x12b7e950, 0x8bbeb8ea, 0xfcb9887c, 0x62dd1ddf, 0x15da2d49, 0x8cd37cf3, 0xfbd44c65,   
         0x4db26158, 0x3ab551ce, 0xa3bc0074, 0xd4bb30e2, 0x4adfa541, 0x3dd895d7, 0xa4d1c46d, 0xd3d6f4fb,   
         0x4369e96a, 0x346ed9fc, 0xad678846, 0xda60b8d0, 0x44042d73, 0x33031de5, 0xaa0a4c5f, 0xdd0d7cc9,   
         0x5005713c, 0x270241aa, 0xbe0b1010, 0xc90c2086, 0x5768b525, 0x206f85b3, 0xb966d409, 0xce61e49f,   
         0x5edef90e, 0x29d9c998, 0xb0d09822, 0xc7d7a8b4, 0x59b33d17, 0x2eb40d81, 0xb7bd5c3b, 0xc0ba6cad,   
         0xedb88320, 0x9abfb3b6, 0x3b6e20c, 0x74b1d29a, 0xead54739, 0x9dd277af, 0x4db2615, 0x73dc1683,   
         0xe3630b12, 0x94643b84, 0xd6d6a3e, 0x7a6a5aa8, 0xe40ecf0b, 0x9309ff9d, 0xa00ae27, 0x7d079eb1,   
         0xf00f9344, 0x8708a3d2, 0x1e01f268, 0x6906c2fe, 0xf762575d, 0x806567cb, 0x196c3671, 0x6e6b06e7,   
         0xfed41b76, 0x89d32be0, 0x10da7a5a, 0x67dd4acc, 0xf9b9df6f, 0x8ebeeff9, 0x17b7be43, 0x60b08ed5,   
         0xd6d6a3e8, 0xa1d1937e, 0x38d8c2c4, 0x4fdff252, 0xd1bb67f1, 0xa6bc5767, 0x3fb506dd, 0x48b2364b,   
         0xd80d2bda, 0xaf0a1b4c, 0x36034af6, 0x41047a60, 0xdf60efc3, 0xa867df55, 0x316e8eef, 0x4669be79,   
         0xcb61b38c, 0xbc66831a, 0x256fd2a0, 0x5268e236, 0xcc0c7795, 0xbb0b4703, 0x220216b9, 0x5505262f,   
         0xc5ba3bbe, 0xb2bd0b28, 0x2bb45a92, 0x5cb36a04, 0xc2d7ffa7, 0xb5d0cf31, 0x2cd99e8b, 0x5bdeae1d,   
         0x9b64c2b0, 0xec63f226, 0x756aa39c, 0x26d930a, 0x9c0906a9, 0xeb0e363f, 0x72076785, 0x5005713,   
         0x95bf4a82, 0xe2b87a14, 0x7bb12bae, 0xcb61b38, 0x92d28e9b, 0xe5d5be0d, 0x7cdcefb7, 0xbdbdf21,   
         0x86d3d2d4, 0xf1d4e242, 0x68ddb3f8, 0x1fda836e, 0x81be16cd, 0xf6b9265b, 0x6fb077e1, 0x18b74777,   
         0x88085ae6, 0xff0f6a70, 0x66063bca, 0x11010b5c, 0x8f659eff, 0xf862ae69, 0x616bffd3, 0x166ccf45,   
         0xa00ae278, 0xd70dd2ee, 0x4e048354, 0x3903b3c2, 0xa7672661, 0xd06016f7, 0x4969474d, 0x3e6e77db,   
         0xaed16a4a, 0xd9d65adc, 0x40df0b66, 0x37d83bf0, 0xa9bcae53, 0xdebb9ec5, 0x47b2cf7f, 0x30b5ffe9,   
         0xbdbdf21c, 0xcabac28a, 0x53b39330, 0x24b4a3a6, 0xbad03605, 0xcdd70693, 0x54de5729, 0x23d967bf,   
         0xb3667a2e, 0xc4614ab8, 0x5d681b02, 0x2a6f2b94, 0xb40bbe37, 0xc30c8ea1, 0x5a05df1b, 0x2d02ef8d,                          
                                   };
        public static int GetCRC32(byte[] bytes)
        {
            int iCount = bytes.Length;
            UInt32 crc = 0xFFFFFFFF;
            for (int i = 0; i < iCount; i++)
            {
                crc = ((crc >> 8) & 0x00FFFFFF) ^ crcTable[(crc ^ bytes[i]) & 0xFF];
            }
            UInt32 temp = crc ^ 0xFFFFFFFF;
            int t = (int)temp;
            return (t);
        }
    }
    #endregion

    #region 一些常量和扩展方法
    /// <summary>
    /// 一些常量和扩展方法
    /// </summary>
    public static class Consts
    {
        /// <summary>
        /// 文件区块数据标头
        /// </summary>
        public const byte FileBlockHeader = 0;
        /// <summary>
        /// 字符串信息标头
        /// </summary>
        public const byte StringHeader = 1;
        /// <summary>
        /// 分块大小1MB
        /// </summary>
        public const int BlockSize = 1048576;
        /// <summary>
        /// 网络上传送的数据包最大大小
        /// </summary>
        public const int NetBlockMaxSize = BlockSize + 9;
        /// <summary>
        /// 默认磁盘缓存大小(单位:区块数)
        /// </summary>
        public const int DefaultIOBufferSize = 8;
        /// <summary>
        /// 空格
        /// </summary>
        public const string Space = " ";
        /// <summary>
        /// 空格替代符
        /// </summary>
        public const string SpaceReplacement = @"<SPACE>";
        /// <summary>
        /// 获取校验值
        /// </summary>
        /// <param name="bytes">输入数据</param>
        /// <returns>输出的校验值</returns>
        public static byte[] GetHash(this byte[] bytes)
        {
            return BitConverter.GetBytes(CRC32.GetCRC32(bytes));
        }
        /// <summary>
        /// 比较两二进制数据内容是否完全相同(用于MD5值的比较)
        /// </summary>
        /// <param name="THIS">数据一</param>
        /// <param name="obj">数据二</param>
        public static bool BytesEqual(this byte[] THIS, byte[] obj)
        {
            if (THIS.Length != obj.Length)
                return false;
            for (int index = 0; index < obj.Length; index++)
            {
                if (THIS[index] != obj[index])
                    return false;
            }
            return true;
        }
        /// <summary>
        /// 将指令字符串转化为二进制数据并添加标头
        /// </summary>
        public static byte[] ToBytes(this string str_input)
        {
            byte[] strdata = Encoding.UTF8.GetBytes(str_input);
            byte[] output = new byte[1 + strdata.Length];
            output[0] = StringHeader;
            System.Array.Copy(strdata, 0, output, 1, strdata.Length);
            return output;
        }
        /// <summary>
        /// 将二进制数据转化为指令字符串
        /// </summary>
        public static string ToFTString(this byte[] bytes_input)
        {
            if (bytes_input[0] != StringHeader)
                throw new FormatException("Bad Header!");
            return Encoding.UTF8.GetString(bytes_input, 1, bytes_input.Length - 1).TrimEnd('\0');
        }
        /// <summary>
        /// 替换可能会对命令解析造成干扰的字符
        /// </summary>
        public static string DoReplace(this string str_input)
        {
            return str_input.Replace(Space, SpaceReplacement);
        }
        /// <summary>
        /// 还原被替换的字符
        /// </summary>
        public static string DeReplace(this string str_input)
        {
            return str_input.Replace(SpaceReplacement, Space);
        }
    }
    #endregion

    #region 一些委托
    public delegate void BlockFinishedEventHandler(object sender, BlockFinishedEventArgs e);
    public delegate void CommandReceivedEventHandler(object sender, CommandReceivedEventArgs e);
    public delegate void FileTransmissionErrorOccurEventHandler(object sender,FileTransmissionErrorOccurEventArgs e);
    public delegate void Delegate_SendBlocks(int Start, int End);
    public delegate void Delegate_Void_Bool(bool logic);
    public delegate int Delegate_Int_Int(int value);
    #endregion

    #region 文件区块类
    public class FileBlockException : Exception
    {
        public enum ErrorCode
        {
            BadHeader,
            BadIndex,
            IllegalFileBlockSize,
            ChecksumError,
        }
        public ErrorCode Code { get; set; }
        public FileBlockException(string message, ErrorCode ErrorCode)
            : base(message)
        {
            Code = ErrorCode;
        }
    }
    /// <summary>
    /// 文件区块类
    /// </summary>
    public class FileBlock : IComparable<FileBlock>
    {
        /// <summary>
        /// 与该区块关联的传输对象
        /// </summary>
        internal FileTransmission _Task;
        /// <summary>
        /// 与该区块关联的FileStream
        /// </summary>
        internal FileStream _FileStream;
        /// <summary>
        /// 文件数据
        /// </summary>
        internal byte[] _Data;
        /// <summary>
        /// 数据长度
        /// </summary>
        internal int _DataLength;
        /// <summary>
        /// 数据的Hash值
        /// </summary>
        internal byte[] _DataHash;
        /// <summary>
        /// 获取或设置该区块的序号(该区块在文件中的位置)
        /// </summary>
        public int Index { get; set; }
        /// <summary>
        /// 获取该区块的数据长度
        /// </summary>
        public int DataLength { get { return _DataLength; } }
        /// <summary>
        /// 获取该数据块的校验值
        /// </summary>
        public byte[] DataHash { get { return _DataHash; } }
        /// <summary>
        /// 构造函数
        /// 用于从文件读入区块
        /// </summary>
        /// <param name="fStream">输入的文件流</param>
        /// <param name="BlockIndex">分块位置</param>
        /// <param name="ReadOnCreated">是否立即从文件读取数据</param>
        public FileBlock(FileTransmission TransmissionTask, int BlockIndex, bool ReadOnCreated)
        {
            _Task = TransmissionTask;
            _FileStream = _Task.FileStream;
            Index = BlockIndex;
            if (ReadOnCreated)
                this.Read(true);
        }
        /// <summary>
        /// 构造函数
        /// 用于从二进制数据读入区块
        /// </summary>
        /// <param name="fStream">要保存的文件流</param>
        /// <param name="ReceivedData">输入的二进制数据</param>
        public FileBlock(FileTransmission TransmissionTask, byte[] ReceivedData)
        {
            _Task = TransmissionTask;
            _FileStream = _Task.FileStream;
            if (ReceivedData[0] != Consts.FileBlockHeader)
                throw new FileBlockException("Bad Header!", FileBlockException.ErrorCode.BadHeader);
            Index = BitConverter.ToInt32(ReceivedData, 1);
            _DataLength = ReceivedData.Length - 9;
            if (_DataLength > Consts.BlockSize)
                throw new FileBlockException("Illegal FileBlock Size!", FileBlockException.ErrorCode.IllegalFileBlockSize);
            _Data = new byte[_DataLength];
            _DataHash = new byte[4];
            System.Array.Copy(ReceivedData, 5, _DataHash, 0, 4);
            System.Array.Copy(ReceivedData, 9, _Data, 0, _DataLength);
            if (!_DataHash.BytesEqual(_Data.GetHash()))
                throw new FileBlockException("Error Hash!", FileBlockException.ErrorCode.ChecksumError);
        }
        /// <summary>
        /// 从文件读入
        /// </summary>
        /// <param name="CalcHashAfterRead">是否在读取后立即计算校验值</param>
        /// <returns>读取块的大小</returns>
        public int Read(bool CalcHashAfterRead)
        {
            _Data = new byte[Consts.BlockSize];
            lock (_FileStream)
            {
                _FileStream.Position = (long)Index * (long)Consts.BlockSize;
                _DataLength = _FileStream.Read(_Data, 0, Consts.BlockSize);
            }
            if (_Data.Length != _DataLength)
            {
                byte[] old = _Data;
                _Data = new byte[_DataLength];
                System.Array.Copy(old, _Data, _DataLength);
            }
            if (CalcHashAfterRead)
                CalcHash();
            return _DataLength;
        }
        /// <summary>
        /// 计算校验值
        /// </summary>
        /// <returns>校验值</returns>
        public byte[] CalcHash()
        {
            return _DataHash = _Data.GetHash();
        }
        /// <summary>
        /// 将该区块写入文件
        /// </summary>
        public void Write()
        {
            lock (_FileStream)
            {
                _FileStream.Position = (long)Index * (long)Consts.BlockSize;
                _FileStream.Write(_Data, 0, _DataLength);
            }
        }
        /// <summary>
        /// 转化为二进制数据以传输
        /// </summary>
        /// <returns></returns>
        public byte[] GetBytes()
        {
            MemoryStream mStream = new MemoryStream(1 + 4 + 4 + _DataLength);
            byte[] Header = new byte[1] { Consts.FileBlockHeader };
            mStream.Write(Header, 0, 1);
            mStream.Write(BitConverter.GetBytes(Index), 0, 4);
            mStream.Write(_DataHash, 0, 4);
            mStream.Write(_Data, 0, _DataLength);
            return mStream.ToArray();
        }
        int System.IComparable<FileBlock>.CompareTo(FileBlock obj)
        {
            return (Index as IComparable<int>).CompareTo(obj.Index);
        }
    }
    #endregion

    #region 事件参数类
    public class BlockFinishedEventArgs : EventArgs
    {
        public int BlockIndex { get; set; }
        public BlockFinishedEventArgs(int BlockIndex) { this.BlockIndex = BlockIndex; }
    }
    public class CommandReceivedEventArgs : EventArgs
    {
        public string CommandLine { get; set; }
        public CommandReceivedEventArgs(string CommandLine) { this.CommandLine = CommandLine; }
    }
    public class FileTransmissionErrorOccurEventArgs : EventArgs
    {
        public Exception InnerException { get; set; }
        /// <summary>
        /// 指示是否继续运行
        /// </summary>
        public bool Continue { get; set; }
        public FileTransmissionErrorOccurEventArgs(Exception innerException)
        {
            InnerException = innerException;
            Continue = false;
        }
    }
    #endregion

    #region 文件区块抽象集合类
    /// <summary>
    /// 文件区块的抽象集合
    /// 之所以说抽象是因为该集合并不存储实际的区块(缓存区除外)
    /// 而是通过一个索引器来读写文件
    /// 并提供磁盘缓存
    /// </summary>
    public class FileBlockCollection
    {
        /// <summary>
        /// 与该区块关联的传输对象
        /// </summary>
        internal FileTransmission _Task;
        /// <summary>
        /// 与该区块关联的FileStream
        /// </summary>
        internal FileStream _FileStream;
        internal bool _EnabledIOBuffer;
        /// <summary>
        /// 磁盘缓存区
        /// </summary>
        internal Dictionary<int, FileBlock> _IOBuffer;

        public FileBlockCollection(FileTransmission TransmissionTask)
        {
            _Task = TransmissionTask;
            _FileStream = _Task.FileStream;
            _IOBufferSize = Consts.DefaultIOBufferSize;
        }
        /// <summary>
        /// 获取或设置一个值,该值指示是否启用磁盘缓存
        /// </summary>
        internal bool EnabledIOBuffer
        {
            get { return _EnabledIOBuffer; }
            set
            {
                _EnabledIOBuffer = value;
                if (value)
                    _IOBuffer = new Dictionary<int, FileBlock>();
                else
                {
                    if (_Task is FileReceiver)
                        WriteAllBlock();
                    _IOBuffer = null;
                }
            }
        }
        internal int _IOBufferSize;
        /// <summary>
        /// 获取已接收或已发送的区块序号列表
        /// </summary>
        public List<int> Finished { get { return _Task._FinishedBlock; } }
        /// <summary>
        /// 获取已存在(Hash成功)的区块序号列表
        /// </summary>
        public List<int> Exist
        {
            get
            {
                if (_Task is FileReceiver)
                    return ((FileReceiver)_Task)._ExistBlock;
                else
                    return null;
            }
        }
        /// <summary>
        /// 获取被丢弃的区块序号列表
        /// </summary>
        public List<int> Cast
        {
            get
            {
                if (_Task is FileReceiver)
                    return ((FileReceiver)_Task)._CastBlock;
                else
                    return null;
            }
        }
        /// <summary>
        /// 获取总区块数
        /// </summary>
        public int Count { get { return _Task._TotalBlock; } }
        /// <summary>
        /// 获取有效区块数(已存在+已接收)
        /// </summary>
        public int CountValid
        {
            get
            {
                if (_Task is FileReceiver)
                    return _Task._FinishedBlock.Count + ((FileReceiver)_Task)._ExistBlock.Count;
                else
                    return _Task._FinishedBlock.Count;

            }
        }
        /// <summary>
        /// 将缓存中的区块全部写入磁盘
        /// </summary>
        /// <returns>写入的区块数量</returns>
        public int WriteAllBlock()
        {
            if (!_EnabledIOBuffer)
                return -1;
            int count = 0;
            lock (_IOBuffer)
            {
                foreach (var b in _IOBuffer)
                {
                    b.Value.Write();
                    count++;
                }
                if (count != _IOBuffer.Count)
                    throw new IOException("Can not Write All FileBlocks!");
                _IOBuffer.Clear();
            }
            return count;
        }
        /// <summary>
        /// 读取数据以填充缓存
        /// </summary>
        /// <param name="StartIndex">起始区块</param>
        /// <returns>读取的区块数量</returns>
        public int FillIOBuffer(int StartIndex)
        {
            int Index;
            lock (_IOBuffer)
            {
                _IOBuffer.Clear();
                for (Index = StartIndex; _IOBuffer.Count < _IOBufferSize && Index < _Task.Blocks.Count; Index++)
                {
                    _IOBuffer.Add(Index, new FileBlock(_Task, Index, true));
                }
            }
            return Index - StartIndex;
        }
        /// <summary>
        /// 异步填充缓存
        /// </summary>
        /// <param name="StartIndex">起始区块</param>
        public IAsyncResult BeginFillIOBuffer(int StartIndex,AsyncCallback callback,object state)
        {
            return new Delegate_Int_Int(FillIOBuffer).BeginInvoke(StartIndex, callback, state);
        }
        /// <summary>
        /// 写入区块
        /// </summary>
        /// <param name="value">区块对象</param>
        public void Write(FileBlock value)
        {
            if (_EnabledIOBuffer)
            {
                if (_IOBuffer.Count >= _IOBufferSize)
                    WriteAllBlock();
                lock (_IOBuffer)
                    _IOBuffer.Add(value.Index, value);
            }
            else
                value.Write();
        }
        /// <summary>
        /// 读取或写入区块
        /// </summary>
        /// <param name="BlockIndex">区块序号</param>
        public FileBlock this[int BlockIndex]
        {
            get
            {
                FileBlock output;
                if (_EnabledIOBuffer)
                {
                    
                    bool IsInBuf;
                    lock (_IOBuffer)
                        IsInBuf = _IOBuffer.TryGetValue(BlockIndex, out output);
                    if (IsInBuf)
                        return output;
                    else
                    {
                        output = new FileBlock(_Task, BlockIndex, true);
                        BeginFillIOBuffer(BlockIndex + 1, null, null);
                    }
                }
                else
                    output = new FileBlock(_Task, BlockIndex, true);
                return output;
            }
            set
            {
                if (BlockIndex != value.Index)
                    throw new FileBlockException("Bad Index!", FileBlockException.ErrorCode.BadIndex);
                Write(value);
            }
        }
    }
    #endregion

    #region 文件传输基类
    public abstract class FileTransmission : IDisposable
    {
        internal FileStream _FileStream;
        //internal readonly TransmissionMode _Mode;
        /// <summary>
        /// 总区块数
        /// </summary>
        internal int _TotalBlock;
        /// <summary>
        /// 最后一个区块的大小
        /// </summary>
        internal int _LastBlockSize;
        internal List<int> _FinishedBlock;
        internal byte[] ReceiveBuf;
        internal Socket _Socket;
        internal EventWaitHandle _WaitHandle;
        internal bool _IsAlive;
        internal FileBlockCollection _Blocks;
        internal DateTime _StartTime;
        /// <summary>
        /// 上一个区块完成的时间
        /// </summary>
        internal DateTime _PriorBlockTime;
        internal double _ByteSpeed;
        /// <summary>
        /// 获取或设置一个值,该值指示是否启用磁盘缓存
        /// </summary>
        public bool EnabledIOBuffer
        {
            get { return _Blocks._EnabledIOBuffer; }
            set { _Blocks.EnabledIOBuffer = value; }
        }
        /// <summary>
        /// 获取或设置磁盘缓存的大小(单位:区块数)
        /// </summary>
        public int IOBufferSize
        {
            get { return _Blocks._IOBufferSize; }
            set
            {
                if (!_Blocks._EnabledIOBuffer)
                    throw new InvalidOperationException("IOBuffer is not enabled!");
                _Blocks._IOBufferSize = value;
            }
        }
        /// <summary>
        /// 获取当前磁盘缓存中的区块数
        /// </summary>
        public int CurrentIOBufferSize
        {
            get
            {
                if (!_Blocks._EnabledIOBuffer)
                    return 0;
                return _Blocks._IOBuffer.Count;
            }
        }
        /// <summary>
        /// 获取或设置该传输的目标连接
        /// </summary>
        public Socket Socket
        {
            get { return _Socket; }
            set
            {
                try
                {
                    if (value.ProtocolType != ProtocolType.Tcp)
                        throw new ArgumentException("Socket Protocol must be TCP", "Socket");
                    _Socket = value;
                    _Socket.ReceiveBufferSize = _Socket.SendBufferSize = Consts.NetBlockMaxSize;
                }
                catch (Exception ex)
                {
                    OnErrorOccurred(ex);
                }
            }
        }
        /// <summary>
        /// 获取与此传输关联的文件流
        /// </summary>
        public FileStream FileStream { get { return _FileStream; } }
        /// <summary>
        /// 获取或设置文件路径
        /// </summary>
        public string FilePath { get; set; }
        /// <summary>
        /// 获取或设置文件名
        /// </summary>
        public string FileName { get; set; }

        //接收保存文件的路径
        public string SaveFilePath { get; set; }
        /// <summary>
        /// 获取或设置文件名(包括路径)，发送时用来读取文件
        /// </summary>
        public string FullFileName
        {
            get
            {
                try
                {
                    return FilePath.TrimEnd('\\') + "\\" + FileName;
                }
                catch (Exception ex)
                {
                    OnErrorOccurred(ex);
                    return null;
                }
            }
            set
            {
                try
                {
                    int i = value.LastIndexOf('\\');
                    if (i > 0)
                        FilePath = value.Substring(0, i);
                    else
                        FilePath = Environment.CurrentDirectory;
                    FileName = value.Substring(i + 1);
                }
                catch (Exception ex)
                {
                    OnErrorOccurred(ex);
                }
            }
        }
        /// <summary>
        /// 一个区块完成时发生
        /// </summary>
        public event BlockFinishedEventHandler BlockFinished;
        /// <summary>
        /// 全部完成时发生
        /// </summary>
        public event EventHandler AllFinished;
        /// <summary>
        /// 连接中断时发生
        /// </summary>
        public event EventHandler ConnectLost;
        /// <summary>
        /// 出现错误时发生
        /// </summary>
        public event FileTransmissionErrorOccurEventHandler ErrorOccurred;
        /// <summary>
        /// 获取一个值,该值指示传输是否正在进行
        /// </summary>
        public bool IsAlive { get { return _IsAlive; } }
        /// <summary>
        /// 获取传输开始的时间
        /// </summary>
        public DateTime StartTime { get { return _StartTime; } }
        /// <summary>
        /// 获取已用时
        /// </summary>
        public TimeSpan TimePast { get { return DateTime.Now - _StartTime; } }
        /// <summary>
        /// 获取估计剩余时间
        /// </summary>
        public abstract TimeSpan TimeRemaining { get; }
        /// <summary>
        /// 获取平均速率(区块/秒)
        /// </summary>
        public double BlockAverSpeed
        {
            get
            {
                return _FinishedBlock.Count / TimePast.TotalSeconds;
            }
        }
        /// <summary>
        /// 获取平均速率(字节/秒)
        /// </summary>
        public double ByteAverSpeed
        {
            get
            {
                return BlockAverSpeed * Consts.BlockSize;
            }
        }
        /// <summary>
        /// 获取平均速率(千字节/秒)
        /// </summary>
        public double KByteAverSpeed
        {
            get
            {
                return ByteAverSpeed / 1024;
            }
        }
        /// <summary>
        /// 获取瞬时速率(字节/秒)
        /// </summary>
        public double ByteSpeed
        {
            get
            {
                return _ByteSpeed;
            }
        }
        /// <summary>
        /// 获取瞬时速率(千字节/秒)
        /// </summary>
        public double KByteSpeed
        {
            get
            {
                return _ByteSpeed / 1024;
            }
        }
        /// <summary>
        /// 获取文件总长度
        /// </summary>
        public long TotalSize
        {
            get
            {
                return (long)(_TotalBlock - 1) * (long)Consts.BlockSize + (long)_LastBlockSize;
            }
        }
        /// <summary>
        /// 获取已完成的数据长度
        /// </summary>
        public abstract long FinishedSize { get; }
        /// <summary>
        /// 获取进度值(%)
        /// </summary>
        public double Progress
        {
            get
            {
                return ((double)FinishedSize / (double)TotalSize) * 100;
            }
        }
        /// <summary>
        /// 获取该传输的区块集合
        /// </summary>
        public FileBlockCollection Blocks { get { return _Blocks; } }
        /// <summary>
        /// 默认构造函数
        /// </summary>
        public FileTransmission()
        {
            _FinishedBlock = new List<int>();
            _WaitHandle = new EventWaitHandle(false, EventResetMode.ManualReset);
            _Blocks = new FileBlockCollection(this);
        }
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="FilePath">文件路径</param>
        /// <param name="FileName">文件名</param>
        public FileTransmission(string FilePath, string FileName)
        {
            _FinishedBlock = new List<int>();
            _WaitHandle = new EventWaitHandle(true, EventResetMode.ManualReset);
            _Blocks = new FileBlockCollection(this);

            this.FilePath = FilePath;
            this.FileName = FileName;
        }
        /// <summary>
        /// 初始化接收缓存
        /// </summary>
        internal void InitializeReceiveBuf()
        {
            try
            {
                ReceiveBuf = new byte[_Socket.ReceiveBufferSize];
            }
            catch (Exception ex)
            {
                OnErrorOccurred(ex);
            }
        }
        /// <summary>
        /// 开始异步接收
        /// </summary>
        internal abstract IAsyncResult BeginReceive();
        /// <summary>
        /// 开始传输
        /// </summary>
        public virtual void Start()
        {
            try
            {
                _IsAlive = true;
                _StartTime = DateTime.Now;
                _WaitHandle.Reset();
            }
            catch (Exception ex)
            {
                OnErrorOccurred(ex);
            }
        }
        /// <summary>
        /// 中止传输
        /// </summary>
        /// <param name="ShutDownSocket">是否关闭Socket</param>
        public virtual void Stop(bool ShutDownSocket)
        {
            try
            {
                _IsAlive = false;
                _FileStream.Close();
                _FileStream = null;
                _WaitHandle.Set();
                if (ShutDownSocket)
                {
                    _Socket.Shutdown(SocketShutdown.Both);
                    _Socket.Close();
                }
            }
            catch (Exception ex)
            {
                OnErrorOccurred(ex);
            }
        }
        /// <summary>
        /// 异步中止传输,不关闭Socket
        /// </summary>
        internal void Stop()
        {
            new Delegate_Void_Bool(Stop).BeginInvoke(false, null, null);
        }
        /// <summary>
        /// 等待传输完成
        /// </summary>
        public bool WaitForExit()
        {
            return _WaitHandle.WaitOne();
        }
        /// <summary>
        /// 等待传输完成
        /// </summary>
        public bool WaitForExit(int millisecondsTimeout, bool exitContext)
        {
            return _WaitHandle.WaitOne(millisecondsTimeout, exitContext);
        }
        /// <summary>
        /// 等待传输完成
        /// </summary>
        public bool WaitForExit(TimeSpan timeout, bool exitContext)
        {
            return _WaitHandle.WaitOne(timeout, exitContext);
        }
        internal virtual void OnBlockFinished(int BlockIndex)
        {
            if (!_FinishedBlock.Exists(a => a == BlockIndex))
                _FinishedBlock.Add(BlockIndex);
            if (BlockIndex == _TotalBlock - 1)
                _ByteSpeed = _LastBlockSize / (DateTime.Now - _PriorBlockTime).TotalSeconds;
            else
                _ByteSpeed = Consts.BlockSize / (DateTime.Now - _PriorBlockTime).TotalSeconds;
            _PriorBlockTime = DateTime.Now;
            if (BlockFinished != null)
                BlockFinished(this, new BlockFinishedEventArgs(BlockIndex));
        }
        internal virtual void OnConnectLost()
        {
            if (!_IsAlive)
                return;
            Stop();
            if (ConnectLost != null)
                ConnectLost(this, new EventArgs());
        }
        /// <summary>
        /// 同步发送字符串
        /// </summary>
        public int SendString(string str)
        {
            try
            {
                return _Socket.EndSend(BeginSendString(str, null, null));
            }
            catch (SocketException)
            {
                OnConnectLost();
                return 0;
            }
            catch (Exception ex)
            {
                OnErrorOccurred(ex);
                return 0;
            }
        }
        /// <summary>
        /// 异步发送字符串并使用默认的回调方法
        /// </summary>
        public void SendStringAsync(string str)
        {
            BeginSendString(str, SendCallback, null);
        }
        /// <summary>
        /// 异步发送字符串并使用指定的的回调方法和参数
        /// </summary>
        public IAsyncResult BeginSendString(string str, AsyncCallback callback, object state)
        {
            try
            {
                if (!_IsAlive)
                    throw new InvalidOperationException("Is Not Alive");
                byte[] ToSend = str.ToBytes();
                return _Socket.BeginSend(ToSend, 0, ToSend.Length, SocketFlags.None, callback, state);
            }
            catch (SocketException)
            {
                OnConnectLost();
                return null;
            }
            catch (Exception ex)
            {
                OnErrorOccurred(ex);
                return null;
            }
        }
        internal void SendCallback(IAsyncResult ar)
        {
            try
            {
                _Socket.EndSend(ar);
            }
            catch (SocketException)
            {
                OnConnectLost();
            }
            catch (Exception ex)
            {
                OnErrorOccurred(ex);
            }
            if (ar.AsyncState != null)
            {
                if (ar.AsyncState is int)
                {
                    OnBlockFinished((int)ar.AsyncState);
                }
            }
        }
        internal virtual void OnAllFinished()
        {
            if (AllFinished != null)
                AllFinished(this, new EventArgs());
        }
        internal virtual void OnErrorOccurred(Exception innerException)
        {
            FileTransmissionErrorOccurEventArgs eventargs = new FileTransmissionErrorOccurEventArgs(innerException);
            if (ErrorOccurred != null)
                ErrorOccurred(this, eventargs);
            if (!eventargs.Continue)
                throw innerException;
        }
        void System.IDisposable.Dispose()
        {
            _FileStream.Close();
            _Socket.Close();
        }
    }
    #endregion

    #region 发送端类
    /// <summary>
    /// 发送端
    /// 传输前发送端创建该类实例
    /// 设置必要属性后
    /// 调用Start()方法开始传输
    /// </summary>
    public class FileSender : FileTransmission
    {
        /// <summary>
        /// 接收到命令时发生
        /// </summary>
        public event CommandReceivedEventHandler CommandReceived;
        /// <summary>
        /// 开始异步接收
        /// </summary>
        internal override IAsyncResult BeginReceive()
        {
            InitializeReceiveBuf();
            try
            {
                return _Socket.BeginReceive(ReceiveBuf, 0, ReceiveBuf.Length, SocketFlags.None, ReceiveCallback, null);
            }
            catch (SocketException)
            {
                OnConnectLost();
                return null;
            }
            catch (Exception ex)
            {
                OnErrorOccurred(ex);
                return null;
            }
        }
        /// <summary>
        /// 开始传输
        /// </summary>
        public override void Start()
        {
            base.Start();
            try
            {
                BeginReceive();
                _FileStream = new FileStream(FullFileName, FileMode.Open, FileAccess.Read, FileShare.Read);
                _TotalBlock = (int)(_FileStream.Length / (long)Consts.BlockSize) + 1;
                _LastBlockSize = (int)(_FileStream.Length - ((long)_TotalBlock - 1) * (long)Consts.BlockSize);
            }
            catch (Exception ex)
            {
                OnErrorOccurred(ex);
            }
        }
        /// <summary>
        /// 获取估计剩余时间
        /// </summary>
        public override TimeSpan TimeRemaining
        {
            get
            {
                int BlockRemaining = _TotalBlock - _FinishedBlock.Count;
                return TimeSpan.FromSeconds(BlockRemaining / BlockAverSpeed);
            }
        }
        /// <summary>
        /// 获取已完成的数据长度
        /// </summary>
        public override long FinishedSize
        {
            get
            {
                return (long)_FinishedBlock.Count * (long)Consts.BlockSize;
            }
        }
        /// <summary>
        /// 同步发送区块
        /// </summary>
        /// <param name="BlockIndex">区块序号</param>
        /// <returns>发送的数据长度</returns>
        public int SendBlock(int BlockIndex)
        {
            try
            {
                int ret = _Socket.EndSend(BeginSendBlock(BlockIndex, null, null));
                OnBlockFinished(BlockIndex);
                return ret;
            }
            catch (SocketException)
            {
                OnConnectLost();
                return 0;
            }
            catch (Exception ex)
            {
                OnErrorOccurred(ex);
                return 0;
            }
        }
        /// <summary>
        /// 异步发送区块并使用默认的回调方法
        /// </summary>
        /// <param name="BlockIndex">区块序号</param>
        public void SendBlockAsync(int BlockIndex)
        {
            BeginSendBlock(BlockIndex, SendCallback, BlockIndex);
        }
        /// <summary>
        /// 异步发送区块并使用指定的回调方法和参数
        /// </summary>
        /// <param name="BlockIndex">区块序号</param>
        public IAsyncResult BeginSendBlock(int BlockIndex, AsyncCallback callback, object state)
        {
            try
            {
                if (!_IsAlive)
                    throw new InvalidOperationException("Is Not Alive");
                if (BlockIndex >= _TotalBlock)
                    throw new ArgumentOutOfRangeException("BlockIndex");
                byte[] ToSend = _Blocks[BlockIndex].GetBytes();
                return _Socket.BeginSend(ToSend, 0, ToSend.Length, SocketFlags.None, callback, state);
            }
            catch (SocketException)
            {
                OnConnectLost();
                return null;
            }
            catch (Exception ex)
            {
                OnErrorOccurred(ex);
                return null;
            }
        }
        internal void ReceiveCallback(IAsyncResult ar)
        {
            if (_Socket.Connected == false)
            {
                return;
            }
            bool ContinueReceive = true;
            int count = 0;
            try
            {
                count = _Socket.EndReceive(ar);
            }
            catch (SocketException)
            {
                OnConnectLost();
                return;
            }
            catch (Exception ex)
            {
                try
                {
                    OnErrorOccurred(ex);
                }
                catch { return; }
            }
            try
            {
                if (count == 0)
                    return;
                switch (ReceiveBuf[0])
                {
                    case Consts.StringHeader:
                        ContinueReceive = OnCommandReceived(ReceiveBuf.ToFTString());
                        break;
                    default:
                        throw new FormatException("Bad Header!");
                }
            }
            catch (Exception ex)
            {
                OnErrorOccurred(ex);
            }
            if (ContinueReceive)
            {
                BeginReceive();
            }
        }
        /// <summary>
        /// 命令处理
        /// </summary>
        /// <param name="str">收到的命令</param>
        /// <returns>是否继续接收</returns>
        internal bool OnCommandReceived(string str)
        {
            if (CommandReceived != null)
                CommandReceived(this, new CommandReceivedEventArgs(str));
            bool ContinueReceive = true;
            string[] Msg = str.Split(' ');
            if (Msg[0] == "Exit")
            {
                OnAllFinished();
                ContinueReceive = false;
                Stop();
            }
            else if (Msg[0] == "GET")
            {
                if (Msg[1] == "FileBlock")
                {
                    int BlockIndex;
                    if (!int.TryParse(Msg[2], out BlockIndex))
                        throw new FormatException("Bad BlockIndex " + Msg[2]);
                    SendBlock(BlockIndex);
                }
                else if (Msg[1] == "BlockHash")
                {
                    int BlockIndex;
                    if (!int.TryParse(Msg[2], out BlockIndex))
                        throw new FormatException("Bad BlockIndex " + Msg[2]);
                    byte[] hash = _Blocks[BlockIndex].DataHash;
                    SendStringAsync(string.Format("BlockHash {0} {1}", BlockIndex, BitConverter.ToInt32(hash, 0)));
                }
                else if (Msg[1] == "FileName")
                {
                    SendStringAsync(string.Format("SET FileName {0}", FileName.DoReplace()));
                }
                else if (Msg[1] == "TotalBlock")
                {
                    SendStringAsync(string.Format("SET TotalBlock {0}", _TotalBlock));
                }
                else if (Msg[1] == "LastBlockSize")
                {
                    SendStringAsync(string.Format("SET LastBlockSize {0}", _LastBlockSize));
                }
                else if (Msg[1] == "SaveFilePath")
                {
                    SendStringAsync(string.Format("SET SaveFilePath {0}", SaveFilePath));
                }
                else
                    throw new FormatException("Bad Command " + Msg[1]);
            }
            else
                throw new FormatException("Bad Command " + Msg[0]);

            return ContinueReceive;
        }
    }
    #endregion

    #region 接收端类
    /// <summary>
    /// 接收端
    /// 传输前接收端创建该类实例
    /// 设置必要属性后
    /// 调用Start()方法开始传输
    /// </summary>
    public class FileReceiver : FileTransmission
    {
        internal List<int> _ExistBlock;
        internal List<int> _CastBlock;
        /// <summary>
        /// 下载线程
        /// </summary>
        internal Thread _DownThread;
        public event BlockFinishedEventHandler BlockHashed;
        /// <summary>
        /// 开始异步接收
        /// </summary>
        internal override IAsyncResult BeginReceive()
        {
            InitializeReceiveBuf();
            try
            {
                return _Socket.BeginReceive(ReceiveBuf, 0, ReceiveBuf.Length, SocketFlags.None, null, null);
            }
            catch (SocketException)
            {
                OnConnectLost();
                return null;
            }
            catch (Exception ex)
            {
                OnErrorOccurred(ex);
                return null;
            }
        }
        /// <summary>
        /// 获取估计剩余时间
        /// </summary>
        public override TimeSpan TimeRemaining
        {
            get
            {
                int BlockRemaining = _TotalBlock - _FinishedBlock.Count - ((FileReceiver)this)._ExistBlock.Count;
                return TimeSpan.FromSeconds(BlockRemaining / BlockAverSpeed);
            }
        }
        /// <summary>
        /// 获取已完成的数据长度
        /// </summary>
        public override long FinishedSize
        {
            get
            {
                return ((long)_FinishedBlock.Count + (long)_ExistBlock.Count - 1) * (long)Consts.BlockSize + (long)_LastBlockSize;
            }
        }
        /// <summary>
        /// 开始传输
        /// </summary>
        public override void Start()
        {
            base.Start();
            try
            {
                _CastBlock = new List<int>();
                _ExistBlock = new List<int>();
                _DownThread = new Thread(DownLoad);
                _DownThread.IsBackground = true;
                _DownThread.Name = "DownThread";
                _DownThread.Start();
            }
            catch (Exception ex)
            {
                OnErrorOccurred(ex);
            }
        }
        /// <summary>
        /// 中止传输
        /// </summary>
        /// <param name="ShutDownSocket">是否关闭Socket</param>
        public override void Stop(bool ShutDownSocket)
        {
            try
            {
                if (_DownThread != null)
                {
                    if ((_DownThread.ThreadState & ThreadState.Running) == ThreadState.Running)
                        _DownThread.Abort();
                }
            }
            catch (Exception ex)
            {
                OnErrorOccurred(ex);
            }
            base.Stop(ShutDownSocket);
        }
        internal string ReceiveString()
        {
            int count = 0;
            try
            {
                count = _Socket.EndReceive(BeginReceive());
            }
            catch (Exception ex)
            {
                OnConnectLost();
                throw ex;
            }
            if (count == 0)
                return null;
            else
                return ReceiveBuf.ToFTString();
        }
        internal FileBlock ReceiveFileBlock()
        {
            MemoryStream mStream = new MemoryStream();
            while (true)
            {
                int count = 0;
                try
                {
                    count = _Socket.EndReceive(BeginReceive());
                    if (count == 0) throw new Exception();
                }
                catch (Exception ex)
                {
                    OnConnectLost();
                    throw ex;
                }
                mStream.Write(ReceiveBuf, 0, count);
                try
                {//接收到正确的区块则返回
                    return new FileBlock(this, mStream.ToArray());
                }
                catch (FileBlockException ex)
                {//接收到不完整或错误的区块,若不完整则继续接收
                    if (mStream.Length >= Consts.NetBlockMaxSize)
                        throw ex;//区块已达到指定大小但仍然错误,则抛出错误
                }
            }
        }
        /// <summary>
        /// 从发送端获取文件名
        /// </summary>
        public void GetFileName()
        {
            while (true)
            {
                SendString("GET FileName");
                string[] Msg = ReceiveString().Split(' ');
                if (Msg[0] == "SET" && Msg[1] == "FileName")
                {
                    FileName = Msg[2];
                    break;
                }
            }
        }

        public void GetSaveFilePath()
        {
            while (true)
            {
                SendString("GET SaveFilePath");
                string[] Msg = ReceiveString().Split(' ');
                if (Msg[0] == "SET" && Msg[1] == "SaveFilePath")
                {
                    SaveFilePath = Msg[2];
                    break;
                }
            }
        }
        /// <summary>
        /// 从发送端获取区块总数
        /// </summary>
        public void GetTotalBlock()
        {
            while (true)
            {
                SendString("GET TotalBlock");
                string[] Msg = ReceiveString().Split(' ');
                if (Msg[0] == "SET" && Msg[1] == "TotalBlock")
                {
                    if (int.TryParse(Msg[2], out _TotalBlock))
                        break;
                }
            }
        }
        /// <summary>
        /// 从发送端获取最后一个区块的大小
        /// </summary>
        public void GetLastBlockSize()
        {
            while (true)
            {
                SendString("GET LastBlockSize");
                string[] Msg = ReceiveString().Split(' ');
                if (Msg[0] == "SET" && Msg[1] == "LastBlockSize")
                {
                    if (int.TryParse(Msg[2], out _LastBlockSize))
                        break;
                }
            }
        }
        /// <summary>
        /// 校验文件
        /// </summary>
        /// <returns>损坏或尚未下载的区块序号列表</returns>
        public List<int> HashFile()
        {
            _FileStream.Position = 0;
            _ExistBlock.Clear();
            for (int count = 0; _FileStream.Position < _FileStream.Length && count < _TotalBlock; count++)
            {//校验已存在的区块
                FileBlock TestBlock = new FileBlock(this, count, true);
                SendString(string.Format("GET BlockHash {0}", count));
                string[] Msg = ReceiveString().Split(' ');
                if (Msg[0] == "BlockHash")
                {
                    if (Convert.ToInt32(Msg[1]) == count)
                    {
                        if (BitConverter.ToInt32(TestBlock.DataHash, 0) == Convert.ToInt32(Msg[2]))
                            _ExistBlock.Add(count);
                    }
                }
                if (BlockHashed != null)
                    BlockHashed(this, new BlockFinishedEventArgs(count));
            }
            int MaxExistBlockIndex;//已存在的区块最大序号
            try
            {
                MaxExistBlockIndex = _ExistBlock.Max();
            }
            catch
            {
                MaxExistBlockIndex = 0;
            }
            List<int> BlockRemaining = new List<int>();
            for (int index = 0; index < _TotalBlock; )
            {//计算仍需传输的区块
                if (index <= MaxExistBlockIndex)
                {
                    if (_ExistBlock.Exists(a => a == index))
                    {
                        index++;
                        continue;
                    }
                }
                BlockRemaining.Add(index++);
            }
            return BlockRemaining;
        }
        /// <summary>
        /// 接收整个文件
        /// </summary>
        internal void DownLoad()
        {
            try
            {
                //if (string.IsNullOrEmpty(FilePath))//未指定路径时默认为接收程序所在路径
                //    FilePath = Environment.CurrentDirectory;
                if (string.IsNullOrEmpty(SaveFilePath))//未指定路径时默认为接收程序所在路径
                {
                    GetSaveFilePath();
                    //FilePath = Environment.CurrentDirectory;
                }
                if (string.IsNullOrEmpty(FileName))//未指定文件名时从发送端获取
                {
                    GetFileName();
                }
                _FileStream = new FileStream(SaveFilePath + "/" + FileName, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None);//temp
                GetTotalBlock();
                GetLastBlockSize();
                List<int> BlockRemaining = HashFile();
                if (_FileStream.Length > TotalSize)//如果已存在的文件比目标文件长则截断它
                    _FileStream.SetLength(TotalSize);
                _StartTime = DateTime.Now;
                foreach (int index in BlockRemaining)
                {
                    FileBlock Block;
                    while (true)
                    {
                        SendString(string.Format("GET FileBlock {0}", index));
                        try
                        {
                            Block = ReceiveFileBlock();
                            break;
                        }
                        catch (FileBlockException)
                        {//接收到错误的区块,抛弃该数据并重新请求
                            _CastBlock.Add(index);
                        }
                        catch (Exception ex)
                        {
                            OnErrorOccurred(ex);
                        }
                    }
                    while (true)
                    {
                        try
                        {
                            _Blocks[index] = Block;//写入区块
                            OnBlockFinished(index);
                            break;
                        }
                        catch (IOException ex)
                        {//磁盘写入错误时
                            try
                            {
                                OnErrorOccurred(ex);
                                //重试
                            }
                            catch
                            {//退出
                                Stop();
                                return;
                            }
                        }
                        catch (Exception ex)
                        {
                            OnErrorOccurred(ex);
                        }
                    }
                }
                SendStringAsync("Exit");
                _Blocks.WriteAllBlock();
                OnAllFinished();
                Stop();
            }
            catch (SocketException) { }
            catch (Exception ex)
            {
                OnErrorOccurred(ex);
            }
        }
    }
    #endregion
}