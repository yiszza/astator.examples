using System.Collections.Generic;
using Android.Graphics.Drawables;

namespace MaterialShowcase;
public static class Utils
{
     public static StateListDrawable AddState(
       this StateListDrawable drawable,
       Drawable sub,
       params List<int> states
    )
    {
       drawable.AddState([.. states], sub);
       return drawable;
    }
}