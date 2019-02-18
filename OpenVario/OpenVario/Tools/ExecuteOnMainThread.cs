using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace OpenVario
{
    /// <summary>
    /// Static function to execute a method on the main thread
    /// </summary>
    class ExecuteOnMainThread
    {
        /// <summary>
        /// Asynchronous method invoke on the main thread
        /// </summary>
        /// <param name="a">Method to be invoked on the main thread</param>
        public static void BeginInvoke(Action a)
        {
            Device.BeginInvokeOnMainThread(a);
        }

        /// <summary>
        /// Synchronous method invoke on the main thread
        /// </summary>
        /// <param name="a">Method to be invoked on the main thread</param>
        public static void Invoke(Action a)
        {
            BeginInvokeAsync(a).Wait();
        }

        /// <summary>
        /// Synchronous method invoke on the main thread
        /// </summary>
        /// <typeparam name="T">Return data type</typeparam>
        /// <param name="f">Method to be invoked on the main thread</param>
        /// <returns>Method's return value</returns>
        public static T Invoke<T>(Func<T> f)
        {
            Task<T> task = BeginInvokeAsync<T>(f);
            task.Wait();
            return task.Result;
        }

        /// <summary>
        /// Waitable task invoke on the main thread
        /// </summary>
        /// <param name="a">Method to be invoked on the main thread</param>
        public static Task<bool> BeginInvokeAsync(Action a)
        {
            var tcs = new TaskCompletionSource<bool>();
            Device.BeginInvokeOnMainThread(() =>
            {
                try
                {
                    a();
                    tcs.SetResult(true);
                }
                catch (Exception ex)
                {
                    tcs.SetException(ex);
                }
            });
            return tcs.Task;
        }

        /// <summary>
        /// Waitable task invoke on the main thread
        /// </summary>
        /// <typeparam name="T">Return data type</typeparam>
        /// <param name="f">Method to be invoked on the main thread</param>
        /// <returns>Waitable task object</returns>
        public static Task<T> BeginInvokeAsync<T>(Func<T> f)
        {
            var tcs = new TaskCompletionSource<T>();
            Device.BeginInvokeOnMainThread(() =>
            {
                try
                {
                    var result = f();
                    tcs.SetResult(result);
                }
                catch (Exception ex)
                {
                    tcs.SetException(ex);
                }
            });
            return tcs.Task;
        }
    }
}
