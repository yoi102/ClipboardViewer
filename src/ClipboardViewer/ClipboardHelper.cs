using System;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows;

namespace Walterlv.Clipboards
{
    /// <summary>
    /// ��ʾ����������ĸ����ࡣ
    /// </summary>
    public static class ClipboardHelper
    {
        private const int DefaultRetryTimes = 3;

        /// <summary>
        /// ��ȡ���������ݣ��ڻ�ȡʧ��ʱ���Զ����ԡ�
        /// </summary>
        /// <param name="retryTimes">����ȡʧ��ʱ�Զ����ԵĴ�����</param>
        /// <returns>�Ӽ������л�ȡ�����ݣ�����������������ݣ���Ϊ null��</returns>
        public static IDataObject? GetDataObject(int retryTimes = DefaultRetryTimes)
        {
            return TryDo(() => Clipboard.GetDataObject(), retryTimes);
        }

        /// <summary>
        /// ���ü��������ݣ�������ʧ��ʱ���Զ����ԡ�
        /// </summary>
        /// <param name="dataObject">Ҫ����������е����ݡ�</param>
        /// <param name="retryTimes">������ʧ��ʱ�Զ����ԵĴ�����</param>
        public static void SetDataObject(IDataObject dataObject, int retryTimes = DefaultRetryTimes)
        {
            TryDo(() => Clipboard.SetDataObject(dataObject), retryTimes);
        }

        public static object? TryGetData(this IDataObject dataObject, Type format, int retryTimes = DefaultRetryTimes)
        {
            return TryDo(() => dataObject.GetData(format), retryTimes);
        }

        public static object? TryGetData(this IDataObject dataObject, string format, int retryTimes = DefaultRetryTimes)
        {
            return TryDo(() => dataObject.GetData(format), retryTimes);
        }

        public static object? TryGetData(this IDataObject dataObject, string format, bool autoConvert,
            int retryTimes = DefaultRetryTimes)
        {
            return TryDo(() => dataObject.GetData(format, autoConvert), retryTimes);
        }

        public static void TrySetData(this IDataObject dataObject, Type format, object data,
            int retryTimes = DefaultRetryTimes)
        {
            TryDo(() => dataObject.SetData(format, data), retryTimes);
        }

        public static void TrySetData(this IDataObject dataObject, object data, int retryTimes = DefaultRetryTimes)
        {
            TryDo(() => dataObject.SetData(data), retryTimes);
        }

        public static void TrySetData(this IDataObject dataObject, string format, object data,
            int retryTimes = DefaultRetryTimes)
        {
            TryDo(() => dataObject.SetData(format, data), retryTimes);
        }

        public static void TrySetData(this IDataObject dataObject, string format, object data, bool autoConvert,
            int retryTimes = DefaultRetryTimes)
        {
            TryDo(() => dataObject.SetData(format, data, autoConvert), retryTimes);
        }

        /// <summary>
        /// ����ִ�м�����������ڷ��� <see cref="ExternalException"/> ʱ�Զ����ԡ�
        /// </summary>
        /// <param name="action">���еļ����������</param>
        /// <param name="retryTimes">������ʧ��ʱ���ԵĴ�����</param>
        private static void TryDo(Action action, int retryTimes = DefaultRetryTimes)
        {
            int loopTimes = retryTimes < 0 ? 1 : retryTimes + 1;
            for (int i = 0; i < loopTimes; i++)
            {
                try
                {
                    action();
                    return;
                }
                catch (ExternalException)
                {
                    Thread.Sleep(10);
                }
            }
        }

        /// <summary>
        /// ����ִ�м�����������ڷ��� <see cref="ExternalException"/> ʱ�Զ����ԡ�
        /// </summary>
        /// <typeparam name="T">����������ķ���ֵ���͡�</typeparam>
        /// <param name="func">���еļ����������</param>
        /// <param name="retryTimes">������ʧ��ʱ���ԵĴ�����</param>
        /// <returns>����������ķ���ֵ��</returns>
        private static T? TryDo<T>(Func<T> func, int retryTimes = DefaultRetryTimes)
            where T : class
        {
            int loopTimes = retryTimes < 0 ? 1 : retryTimes + 1;
            for (int i = 0; i < loopTimes; i++)
            {
                try
                {
                    return func();
                }
                catch (ExternalException)
                {
                    Thread.Sleep(10);
                }
            }
            return default;
        }
    }
}