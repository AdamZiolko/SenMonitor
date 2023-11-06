using Android.App;
using Android.Content.PM;
using Android.Hardware;
using Android.Media;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using AndroidX.Fragment.App;
using Google.Android.Material.BottomNavigation;
using System;
using AndroidX.AppCompat.App;
using Android.Content.Res;
using Google.Android.Material.FloatingActionButton;
using Android.Views.Animations;

namespace SenMonitor
{
    [Activity(Label = "SenMonitor", MainLauncher = true)]
    public class MainActivity : AppCompatActivity//, BottomNavigationView.IOnNavigationItemSelectedListener
    {
        private SensorManager _sensorManager;
        private DatabaseManager _databaseManager;

        private TextView _accelerometerDataTextView;
       // private TextView _volumeLevelTextView;
        private TextView _heartRateTextView; // Deklaracja TextView dla tętna
        private TextView _daneZBazy;
        private AccelerometerHandler _accelerometerHandler;
        //private AudioRecorder _audioRecorder;
        private HeartRateSensorHandler _heartRateSensorHandler; // Dodanie obsługi czujnika tętna




        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            SetContentView(Resource.Layout.activity_main);

            _sensorManager = (SensorManager)GetSystemService(SensorService);
            _databaseManager = new DatabaseManager(this);

            _accelerometerDataTextView = FindViewById<TextView>(Resource.Id.accelerometerDataTextView);
            // _volumeLevelTextView = FindViewById<TextView>(Resource.Id.volumeLevelTextView);
            _heartRateTextView = FindViewById<TextView>(Resource.Id.txtHeartRate); // Inicjalizacja TextView dla tętna

            _accelerometerHandler = new AccelerometerHandler(_sensorManager, _accelerometerDataTextView, _databaseManager);
            // _audioRecorder = new AudioRecorder(_volumeLevelTextView);
            _heartRateSensorHandler = new HeartRateSensorHandler(_sensorManager, _heartRateTextView); // Inicjalizacja obsługi czujnika tętna

            //////////////// Baza danych ///////////////////////////////////////////////////////////////////////////////////////////////
            //string dataToSave = "Twoje dane do zapisania";
            //_databaseManager.InsertSensorData(dataToSave);
            //_databaseManager.InsertSensorData("Dane z bazy danych");
            //string latestData = _databaseManager.GetLatestSensorData();
            //_daneZBazy = FindViewById<TextView>(Resource.Id.daneZBazy);
            //_daneZBazy.Text = latestData;
            _databaseManager.ClearAllData();

            //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

            //////////////// Menu Zakładkowe /////////////////////////////////////////////////////


            //BottomNavigationView navigation = FindViewById<BottomNavigationView>(Resource.Id.navigation);
            //navigation.SetOnNavigationItemSelectedListener(this);



            SetupFloatingActionButtonMenu();


        }

        private void SetupFloatingActionButtonMenu()
        {
            var fab = FindViewById<FloatingActionButton>(Resource.Id.fab);
            bool isMenuOpen = false; // Zmienna śledząca stan menu


            fab.Click += (sender, e) =>
            {
                Console.WriteLine(_heartRateTextView.Text);

                fab.Animate().Alpha(0.0f).SetDuration(300);
                if (!isMenuOpen) // Jeśli menu jest zamknięte
                { // Ładuj niestandardowy widok z menu XML
                    View popupView = LayoutInflater.Inflate(Resource.Layout.custom_menu_item, null);
                    PopupWindow popupWindow = new PopupWindow(popupView, ViewGroup.LayoutParams.WrapContent, ViewGroup.LayoutParams.WrapContent);


                    int[] location = new int[2];
                    fab.GetLocationOnScreen(location);
                    int x = location[0] - popupWindow.Width; // Wyśrodkuj w poziomie
                    int y = location[1] - popupWindow.Height; // Przesuń menu w górę
                    Console.WriteLine(location[0]);
                    popupWindow.ShowAtLocation(fab, GravityFlags.CenterHorizontal, 0, -60);

                    // Przygotowanie popupView do animacji początkowej
                    popupView.Alpha = 0.0f;
                    popupView.ScaleX = 0.0f; // Skalowanie X na zero
                    popupView.ScaleY = 0.0f; // Skalowanie Y na zero
                    popupView.TranslationX = x;
                    popupView.TranslationY = -y; // Przesunięcie na początek bliżej środka ekranu

                    // Ustawienie punktu odniesienia (pivot point) w środku popupView
                    popupView.PivotX = 0;
                    popupView.PivotY = popupView.Height / 2;

                    // Rozpoczęcie animacji
                    var animator = popupView.Animate();
                    animator.Alpha(1.0f)
                            .ScaleX(1.0f)
                            .ScaleY(1.0f)
                            .TranslationX(0)
                            .TranslationY(0)
                            .SetDuration(500); // Wydłuż czas trwania animacji na 1 sekundę

                    // Dodanie efektu spowolnienia (jedno odbicie)
                    animator.SetInterpolator(new DecelerateInterpolator());

                    // Obsługa kliknięcia opcji
                    popupView.FindViewById(Resource.Id.navigation_home).Click += (s, args) =>
                    {
                        SupportFragmentManager.PopBackStack(null, (int)Android.App.PopBackStackFlags.Inclusive);
                        popupWindow.Dismiss();
                        fab.Animate().Alpha(1.0f).SetDuration(300);
                        isMenuOpen = false;
                    };

                    // Obsługa innych opcji podobnie jak powyżej
                    popupView.FindViewById(Resource.Id.navigation_page2).Click += (s, args) =>
                    {
                        ShowFragment(new Page2Fragment());
                        popupWindow.Dismiss();
                        fab.Animate().Alpha(1.0f).SetDuration(300);
                        isMenuOpen = false;

                    };

                    popupView.FindViewById(Resource.Id.navigation_page3).Click += (s, args) =>
                    {
                        ShowFragment(new Page3Fragment());
                        popupWindow.Dismiss();
                        fab.Animate().Alpha(1.0f).SetDuration(300);
                        isMenuOpen = false;
                    };

                    popupView.FindViewById(Resource.Id.navigation_page4).Click += (s, args) =>
                    {
                        ShowFragment(new Page4Fragment());
                        popupWindow.Dismiss();
                        fab.Animate().Alpha(1.0f).SetDuration(300);
                        isMenuOpen = false;
                    };

                    popupView.FindViewById(Resource.Id.navigation_page5).Click += (s, args) =>
                    {
                        ShowFragment(new Page5Fragment());
                        popupWindow.Dismiss();
                        fab.Animate().Alpha(1.0f).SetDuration(300);
                        isMenuOpen = false;
                    };

                    // Obsługa zamykania PopupWindow po kliknięciu w inne miejsce
                    Window.DecorView.RootView.Click += (s, args) =>
                    {
                        popupWindow.Dismiss();
                    };
                    isMenuOpen = true;
                }
            };

            void ShowFragment(AndroidX.Fragment.App.Fragment fragment)
            {
                if (fragment != null)
                {
                    SupportFragmentManager.PopBackStack(); // Usuń poprzedni fragment ze stosu wstecz
                    SupportFragmentManager.BeginTransaction()
                        .Replace(Resource.Id.container, fragment)
                        .AddToBackStack(null)
                        .Commit();
                }
            }
        }


        protected override void OnResume()
        {
            base.OnResume();
            //BottomNavigationView navigation = FindViewById<BottomNavigationView>(Resource.Id.navigation);
            //navigation.SetOnNavigationItemSelectedListener(this);
            // to jest dodane jak wyjdzie i wejdzie

            _accelerometerHandler.StartListening();
           // _audioRecorder.StartRecording();
            _heartRateSensorHandler.StartListening(); // Rozpocznij nasłuchiwanie czujnika tętna
        }

        protected override void OnPause()
        {
            base.OnPause();

            _accelerometerHandler.StopListening();
           // _audioRecorder.StopRecording();
            _heartRateSensorHandler.StopListening(); // Zatrzymaj nasłuchiwanie czujnika tętna
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }
        
    }


    

    public class Page2Fragment : AndroidX.Fragment.App.Fragment
    {
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            return inflater.Inflate(Resource.Layout.fragment_my, container, false);
        }
    }

    public class Page3Fragment : AndroidX.Fragment.App.Fragment
    {
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            return inflater.Inflate(Resource.Layout.fragment_my2, container, false);
        }
    }

    public class Page4Fragment : AndroidX.Fragment.App.Fragment
    {
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            return inflater.Inflate(Resource.Layout.fragment_my3, container, false);
        }
    }

    public class Page5Fragment : AndroidX.Fragment.App.Fragment
    {
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            return inflater.Inflate(Resource.Layout.fragment_my4, container, false);
        }
    }
}