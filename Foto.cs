using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android.Provider;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;
using Android.Graphics;
using Android.Util;

using Java.IO;
using System.Collections;
using System.Data;
using SQLite.Net;
using SQLite.Net.Platform.XamarinAndroid;
using Android.Database.Sqlite;

using Android.Content.PM;
using Environment = Android.OS.Environment;
using Uri = Android.Net.Uri;
using Android.Runtime;


namespace MasterA
{
    [Activity(Label = "Fotos Talhão")]

    public class Foto : Fragment
    {
        public static File _file;
        public static File _dir;
        public static Bitmap bitmap;
        ImageView imageView;







        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

        }


      


        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            base.OnCreateView(inflater, container, savedInstanceState);

            var view = inflater.Inflate(Resource.Layout.Tabfoto, container, false);

            //if (IsThereAnAppToTakePictures())
            //{
                CreateDirectoryForPictures();

                Button button = view.FindViewById<Button>(Resource.Id.myButton);
                imageView = view.FindViewById<ImageView>(Resource.Id.imageView1);
               //Button Buscar = view.FindViewById<Button>(Resource.Id.btnBuscaFoto);
                button.Click += TakeAPicture;
                  //Buscar.Click += BuscarFoto;

  



            return view;
        }



        private void CreateDirectoryForPictures()
        {
            _dir = new File(
                Android.OS.Environment.GetExternalStoragePublicDirectory(
                Android.OS.Environment.DirectoryPictures), "CameraAppDemo");
            if (!_dir.Exists())
            {
                _dir.Mkdirs();
            }
        }


        //private bool IsThereAnAppToTakePictures()
        //{
        //    Intent intent = new Intent(MediaStore.ActionImageCapture);
        //    IList<ResolveInfo> availableActivities =
        //        PackageManager.QueryIntentActivities(intent, PackageInfoFlags.MatchDefaultOnly);
        //    return availableActivities != null && availableActivities.Count > 0;
        //}




        private void TakeAPicture(object sender, EventArgs eventArgs)
        {
            Intent intent = new Intent(MediaStore.ActionImageCapture);
            Foto._file = new File(Foto._dir, String.Format("myPhoto_{0}.jpg", Guid.NewGuid()));
            intent.PutExtra(MediaStore.ExtraOutput, Uri.FromFile(Foto._file));
            StartActivityForResult(intent, 0);
        }

      


        public override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);

            // Make it available in the gallery

            Intent mediaScanIntent = new Intent(Intent.ActionMediaScannerScanFile);
            Uri contentUri = Uri.FromFile(Foto._file);
            mediaScanIntent.SetData(contentUri);
            //SendBroadcast(mediaScanIntent);

            // Display in ImageView. We will resize the bitmap to fit the display.
            // Loading the full sized image will consume to much memory
            // and cause the application to crash.

            int height = Resources.DisplayMetrics.HeightPixels;
            int width = imageView.Height;
            NewMethod(height, width);
            //if (Foto.bitmap != null)
            //{
            imageView.SetImageBitmap(Foto.bitmap);
            Foto.bitmap = null;
            //}

            // Dispose of the Java side bitmap.
            GC.Collect();
        }

        private static void NewMethod(int height, int width)
        {
            bitmap = _file.Path.LoadAndResizeBitmap(width, height);
        }

        public static class BitmapHelpers
        {
            public static Bitmap LoadAndResizeBitmap( string fileName, int width, int height)
            {
                // First we get the the dimensions of the file on disk
                BitmapFactory.Options options = new BitmapFactory.Options { InJustDecodeBounds = true };
                BitmapFactory.DecodeFile(fileName, options);

                // Next we calculate the ratio that we need to resize the image by
                // in order to fit the requested dimensions.
                int outHeight = options.OutHeight;
                int outWidth = options.OutWidth;
                int inSampleSize = 1;

                if (outHeight > height || outWidth > width)
                {
                    inSampleSize = outWidth > outHeight
                                       ? outHeight / height
                                       : outWidth / width;
                }

                // Now we will load the image and have BitmapFactory resize it for us.
                options.InSampleSize = inSampleSize;
                options.InJustDecodeBounds = false;
                Bitmap resizedBitmap = BitmapFactory.DecodeFile(fileName, options);

                return resizedBitmap;
            }
        }

    }
}