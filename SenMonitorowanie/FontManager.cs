using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android.App;
using Android.Graphics;
using Android.Util;

namespace SenMonitorowanie
{
    public static class ViewHelper
    {
        public static void SetFontForAllViews(View view, Context context)
        {
            string fontPath = AppSettings.CurrentFontPath;

            if (view is ViewGroup viewGroup)
            {
                int childCount = viewGroup.ChildCount;

                for (int i = 0; i < childCount; i++)
                {
                    Android.Views.View childView = viewGroup.GetChildAt(i);

                    if (childView is Button button)
                    {
                        SetFontForButton(button, context, fontPath);
                    }
                    else if (childView is TextView textView)
                    {
                        SetFontForTextView(textView, context, fontPath);
                    }


                    if (childView is ViewGroup)
                    {
                        SetFontForAllViews((ViewGroup)childView, context);
                    }
                }
            }
        }

        private static void SetFontForButton(Button button, Context context, string fontPath)
        {
            Typeface typeface = Typeface.CreateFromAsset(context.Assets, fontPath);
            button.Typeface = typeface;
        }

        private static void SetFontForTextView(TextView textView, Context context, string fontPath)
        {
            Typeface typeface = Typeface.CreateFromAsset(context.Assets, fontPath);
            textView.Typeface = typeface;
        }
    }


    public class AppSettings
    {
        private static string _currentFontPath;

        public static string CurrentFontPath
        {
            get => _currentFontPath ?? "fonts/RobotoCondensed-Regular.ttf"; // Domyślna czcionka, jeśli nie ustawiono innej
            set => _currentFontPath = value;
        }
    }


}