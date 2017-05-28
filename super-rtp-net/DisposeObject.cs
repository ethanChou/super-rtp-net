using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace super.rtp.net
{
    public class DisposeObject : IDisposable
    {
        private bool disposed = false;

        public void Dispose()
        {
            //必须为true
            Dispose(true);
            //通知垃圾回收机制不再调用终结器（析构器）
            GC.SuppressFinalize(this);
        }

        ~DisposeObject()
        {
            Dispose(false);
        }

        /// <summary>
        /// 非密封类修饰用protected virtual
        /// 密封类修饰用private
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposed)
            {
                return;
            }
            if (disposing)
            {
                // 清理托管资源
               
            }
            disposed = true;
        }

    }
}
