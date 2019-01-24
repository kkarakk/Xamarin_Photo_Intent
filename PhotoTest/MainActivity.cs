using System;
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android.Support.V7.App;
using Plugin.CurrentActivity;
using Plugin.Media;
using Android.Glide;
using Java.IO;
using Xamarin.Essentials;
using Plugin.Media.Abstractions;

namespace PhotoTest
{
	[Activity(Label = "@string/app_name", MainLauncher = true, LaunchMode = Android.Content.PM.LaunchMode.SingleTop, Icon = "@drawable/icon")]
	public class MainActivity : AppCompatActivity
	{

		int count = 1;
		ImageView imageView;
		protected override void OnCreate(Bundle bundle)
		{
			base.OnCreate(bundle);
			SetContentView(Resource.Layout.main);
			Xamarin.Essentials.Platform.Init(this, bundle);
			var toolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar);
			if (toolbar != null)
			{
				SetSupportActionBar(toolbar);
				SupportActionBar.SetDisplayHomeAsUpEnabled(false);
				SupportActionBar.SetHomeButtonEnabled(false);
			}
			CrossCurrentActivity.Current.Init(this, bundle);
			// Get our button from the layout resource,
			// and attach an event to it
			var clickButton = FindViewById<Button>(Resource.Id.my_button);
			imageView = FindViewById<ImageView>(Resource.Id.imageView1);
			clickButton.Click += async (sender, args) =>
			  {
				  var request = new GeolocationRequest(GeolocationAccuracy.High);
				  var location = await Geolocation.GetLastKnownLocationAsync();
				  await CrossMedia.Current.Initialize();
				  if (!CrossMedia.Current.IsCameraAvailable || !CrossMedia.Current.IsTakePhotoSupported)
				  {
					  clickButton.Text = string.Format("No Camera", ":( No camera available.", "OK");
					  return;
				  }

				  var file = await CrossMedia.Current.TakePhotoAsync(new Plugin.Media.Abstractions.StoreCameraMediaOptions
				  {
					  SaveToAlbum = true,
					  Directory = "Sample",
					  Name = "test.jpg",
					  CompressionQuality = 92,
					  PhotoSize = PhotoSize.Small,
					  AllowCropping = true,
					  DefaultCamera = Plugin.Media.Abstractions.CameraDevice.Front
				  });

				  if (file == null)
					  return;
				  File javafile = new File(file.Path);
				  Glide.With(this).Load(javafile).Into(imageView);
				  //imageView.s

				  clickButton.Text = string.Format("{0} clicks!", count++);
			  };

		}

		public override void OnRequestPermissionsResult(int requestCode, string[] permissions, Android.Content.PM.Permission[] grantResults)
		{
			Plugin.Permissions.PermissionsImplementation.Current.OnRequestPermissionsResult(requestCode, permissions, grantResults);
			Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);
			base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
		}
	}
}

