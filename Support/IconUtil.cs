////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Description:
//  Implements the icon image class
//
// Author:
//  Robert C. Steiner
//
// History:
//  =====================================================================================================
//     Date      Who                           What
//  ----------- ------   --------------------------------------------------------------------------------
//   6/15/2015   rcs     Initial Implementation
//  =====================================================================================================
//
// Copyright:
//  BSD 3-Clause License
//  Copyright (c) 2020, Robert C. Steiner
//  All rights reserved.
//
//  Redistribution and use in source and binary forms, with or without
//  modification, are permitted provided that the following conditions are met:
//  1. Redistributions of source code must retain the above copyright notice, this list of conditions and the following
//  disclaimer.
//
//  2. Redistributions in binary form must reproduce the above copyright notice, this list of conditions and the
//  following disclaimer in the documentation and/or other materials provided with the distribution.
//
//  3. Neither the name of the copyright holder nor the names of its
//  contributors may be used to endorse or promote products derived from
//  this software without specific prior written permission.
//
//  THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES,
//  INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
//  DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL,
//  SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR
//  SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY,
//  WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE
//  USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.Reserved.
// 
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace ZCore
{
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>
    /// Icon images for use by controls.
    /// </summary>
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    public static class IconUtil
    {
        //public static Geometry PinnedGeometry   = Geometry.Parse( "F1M4,8L4,5 1,5 1,4 2,4 2,1 2,0 7,0 7,1 7,4 8,4 8,5 5,5 5,8 4,8z M3,1L3,4 5,4 5,1 3,1z" );
        //public static Geometry UnpinnedGeometry = Geometry.Parse( "F1M0,4L3,4 3,1 4,1 4,2 7,2 8,2 8,7 7,7 4,7 4,8 3,8 3,5 0,5 0,4z M7,3L4,3 4,5 7,5 7,3z" );
        //public static Geometry CloseGeometry    = Geometry.Parse( "M0,0 L8,8 M8,0 L0,8" );
        //public static Geometry LockedGeometry   = Geometry.Parse( "F1M0,0L2,0 5,3 8,0 10,0 6,4 10,8 8,8 5,5 2,8 0,8 4,4 0,0z" );
        //public static Geometry UnlockedGeometry = Geometry.Parse( "F1M0,0L2,0 5,3 8,0 10,0 6,4 10,8 8,8 5,5 2,8 0,8 4,4 0,0z" );

        public static Dictionary<string, ImageSource> Images;// = GetResourcesUnderFolder("views/images", Assembly.GetExecutingAssembly());

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Loads the images.
        /// </summary>
        /// <param name="assembly">     . </param>
        /// <param name="viewsImages">  The views images. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public static void Load(Assembly assembly, string viewsImages)
        {
            Images = GetResourcesUnderFolder(viewsImages, assembly);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Gets an image.
        /// </summary>
        /// <param name="key">  The key. </param>
        /// <returns>
        /// The image.
        /// </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public static ImageSource GetImage(string key)
        {
            ImageSource s;
            return Images.TryGetValue(key, out s) ? s : GetSmallIcon(key);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Gets an image key.
        /// </summary>
        /// <param name="path"> . </param>
        /// <returns>
        /// The image key.
        /// </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public static string GetImageKey(string path)
        {
            return path!=null? Path.GetFileNameWithoutExtension(path).ToLower():"unknown";
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Adds an image to 'image'.  Not added if already present.
        /// </summary>
        /// <param name="key">      The key. </param>
        /// <param name="image">    The image. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public static void AddImage(string key, BitmapImage image)
        {
            key = key.ToLower();
            if (!Images.ContainsKey(key))
            {
                Images.Add(key, image);
            }
        }


        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Get small icon image from path.
        /// </summary>
        /// <param name="filePath"> . </param>
        /// <returns>
        /// Icon.
        /// </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public static ImageSource GetSmallIcon(string filePath)
        {
            if (string.IsNullOrEmpty(filePath)) return null;

            var fileExtension = FileUtil.GetFileExtension(filePath).ToLower();

            ImageSource imageSource=null;
            try
            {
                imageSource = GetSystemIcon(filePath);

                if (imageSource != null)
                {
                    if (!Images.ContainsKey(fileExtension))
                    {
                        Images.Add(fileExtension,imageSource);
                    }
                }
            }
            catch
            {
            }
            return imageSource;
        }

         private static ShFileInfo _info;
         const uint SHGFI_ICON              = 0x100;
         const uint SHGFI_LARGEICON         = 0x0;    // 'Large icon
         const uint SHGFI_SMALLICON         = 0x1;    // 'Small icon
         const uint SHGFI_USEFILEATTRIBUTES = 0x10;   // don't test for file exists
     
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Use this to get the large Icon.
        /// </summary>
        /// <param name="path"> . </param>
        /// <returns>
        /// Icon.
        /// </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public static ImageSource GetLargeIcon(string path)
        {
            SHGetFileInfo( path, 0, ref _info, (uint)Marshal.SizeOf( _info ), SHGFI_ICON | SHGFI_LARGEICON | SHGFI_USEFILEATTRIBUTES );
            return Icon.FromHandle( _info.IconHandle ).ToImageSource();
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Converts an icon to an image source.
        /// </summary>
        /// <exception cref="Win32Exception">   Thrown when win32. </exception>
        /// <param name="icon"> The icon. </param>
        /// <returns>
        /// icon as an ImageSource.
        /// </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public static ImageSource ToImageSource(this Icon icon)
        {
            Bitmap bitmap  = icon.ToBitmap();
            IntPtr hBitmap = bitmap.GetHbitmap();

            var result = Imaging.CreateBitmapSourceFromHBitmap( hBitmap, IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions() );
            DeleteObject( hBitmap );

            return result;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Converts a bitmap to a bitmap source.
        /// </summary>
        /// <param name="source">   Source for the. </param>
        /// <returns>
        /// source as a BitmapSource.
        /// </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public static ImageSource ToImageSource(this Bitmap source)
        {
            var hBitmap = source.GetHbitmap();
            var result = Imaging.CreateBitmapSourceFromHBitmap( hBitmap, IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions() );
            DeleteObject( hBitmap );

            return result;
        }


        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Loads an image from a file and adds to the dictionary with the file name as the key.
        /// </summary>
        /// <param name="filePath">Full file path of image </param>
        /// <returns>
        /// The image or null if it does not exist.
        /// </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public static ImageSource LoadImage(string filePath)
        {
            string key = GetImageKey(filePath);
            var image  = GetImage(key);

            if (image == null && FileUtil.FileExists(filePath))
            {
                var bitmap = new BitmapImage(new Uri(filePath));
                bitmap.Freeze();
                AddImage(key,bitmap);
                return bitmap;
            }
            return image;
        }


        //private static ImageSource GetImage(string channel)
        //{
        //    StreamResourceInfo sri = Application.GetResourceStream( new Uri( "/TestApp;component/" + channel, UriKind.Relative ) );
        //    BitmapImage bmp = new BitmapImage();
        //    bmp.BeginInit();
        //    bmp.StreamSource = sri.Stream;
        //    bmp.EndInit();

        //    return bmp;
        //}
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Gets the resources under a folder stored in the application resources.  The file name is the key in the dictionary.
        /// </summary>
        /// <param name="folder">   Pathname of the folder. </param>
        /// <param name="assembly"></param>
        /// <returns>
        /// The dictionary with the images loaded.  
        /// </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public static Dictionary<string, ImageSource> GetResourcesUnderFolder(string folder, Assembly assembly)
        {
            var assemblyName = assembly.GetName().Name;
            var stream       = assembly.GetManifestResourceStream( assemblyName + ".g.resources" );
            var images       = new Dictionary<string, ImageSource>( 80,StringComparer.InvariantCultureIgnoreCase );
         
            if (stream != null)
            {
                var resourceReader = new ResourceReader( stream );
                folder             = folder.ToLower() + "/";
                foreach (var r in resourceReader.OfType<DictionaryEntry>())
                {
                    string path = (string) r.Key;
                    if (path.StartsWith(folder))
                    {
                        var imageKey = GetImageKey( path );
                        if (!images.ContainsKey(imageKey))
                        {
                            var rpath = string.Format( "pack://application:,,,/{0};component/{1}", assemblyName, path );
                            var image = new BitmapImage( new Uri( rpath) );
                            images.Add( imageKey, image );
                        }
                    }
                }
                //get the names
                //var resources =
                //    resourceReader.OfType<DictionaryEntry>()
                //        .Select(p => new {p, theme = (string) p.Key})
                //        .Where(@t => @t.theme.StartsWith(folder))
                //        .Select(@t => @t.theme);
                //   // select theme.Substring( folder.Length );

                //return resources.ToArray();
            }
            stream.Close();
            return images;
        }

        #region Win32

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Windows structure used for Getting icons from the shell.
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        [StructLayout( LayoutKind.Sequential )]
        private struct ShFileInfo
        {
            internal readonly IntPtr IconHandle;
            private readonly int    IconIndex;
            private readonly uint   Attributes;

            /// <summary>
            /// The display name
            /// </summary>
            [MarshalAs( UnmanagedType.ByValTStr, SizeConst = 260 )]
            private readonly string DisplayName;

            /// <summary>
            /// The type name
            /// </summary>
            [MarshalAs( UnmanagedType.ByValTStr, SizeConst = 80 )]
            private readonly string TypeName;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Gets a system icon.
        /// </summary>
        /// <param name="filePath"> file path to get an icon for. </param>
        /// <param name="imageSource">  The image source. </param>
        /// <returns>
        /// The system icon.
        /// </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private static ImageSource GetSystemIcon(string filePath)
        {
            SHGetFileInfo( filePath, 0, ref _info, (uint)Marshal.SizeOf( _info ),SHGFI_ICON | SHGFI_SMALLICON | SHGFI_USEFILEATTRIBUTES );
            Icon fromHandle = Icon.FromHandle( _info.IconHandle );
            var imageSource = fromHandle.ToImageSource();
            return imageSource;
        }


        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Sh get file information.
        /// </summary>
        /// <param name="path">             Geometry of the file </param>
        /// <param name="fileAttributes">   The file attributes. </param>
        /// <param name="shellInfo">        [in,out] Information describing the shell. </param>
        /// <param name="sizeFileInfo">     Information describing the size file. </param>
        /// <param name="flags">            The flags. </param>
        /// <returns>
        /// the image handle and information.
        /// </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        [DllImport( "shell32.dll" )]
        private static extern IntPtr SHGetFileInfo(string path, uint fileAttributes, ref ShFileInfo shellInfo, uint sizeFileInfo, uint flags);

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Deletes the object described by handle.
        /// </summary>
        /// <param name="handle">   The handle. </param>
        /// <returns>
        /// flags
        /// </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        [DllImport( "gdi32" )]
        private static extern int DeleteObject(IntPtr handle);

        #endregion

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Map image (adds map for new key to the same as old key)  Use for extensions etc.
        /// </summary>
        /// <param name="imageKey"> The image key. </param>
        /// <param name="newKey">   The new key. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public static void MapImage(string imageKey, string newKey)
        {
            var image = GetImage(imageKey);
            var key   = newKey.ToLower();
         
            if (!Images.ContainsKey( key ))
            {
                Images.Add( key, image );
            }
        }
    }
}