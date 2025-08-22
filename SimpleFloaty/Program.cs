using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Android.Animation;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Astator.Platform.Droid.Permissions;
using Astator.Platform.Droid.Script;
using Astator.Platform.Droid.UI;
using Astator.Platform.Droid.UI.Base;
using Astator.Script;
using Microsoft.Maui.Graphics;
using Microsoft.Maui.Platform;
using static Android.Views.ViewGroup;
using static Android.Widget.ImageView;

namespace SimpleFloaty;

public class Program
{
    private static readonly TaskCompletionSource<bool> tcs = new();
    private static TemplateActivity activity;
    private static FloatyWindow menuFloaty;
    private static FloatyWindow subFloaty;

    private static bool isMoving;
    private static double floatyX;
    private static double floatyY;
    private static double downX;
    private static double downY;

    [EntryMethod]
    public static async Task<bool> MainAsync(string workDir)
    {
        activity = await TemplateActivity.CreateAsync();
        var helper = PermissionHelper.CreateInstance(activity);

        activity.SetContentViewDelegate(
            workDir,
            (ctx, workDir) =>
            {
                var root = new LinearLayout(ctx)
                    .Orientation(Orientation.Vertical)
                    .LayoutParams(
                        new LayoutParams(LayoutParams.MatchParent, LayoutParams.MatchParent)
                    )
                    .Children(
                        new TextView(ctx)
                            .Text("悬浮窗示例")
                            .TextSize(20)
                            .Padding(24, 12, 24, 12)
                            .TextStyle(Android.Graphics.TypefaceStyle.Bold)
                            .Gravity(GravityFlags.Left),
                        new Button(ctx)
                            .Text("开启悬浮窗")
                            .Margin(20, 5, 20, 5)
                            .OnClick(async view =>
                            {
                                if (!helper.CheckFloaty())
                                {
                                    await helper.ReqFloatyAsync();
                                    return;
                                }

                                if (menuFloaty == null)
                                    CreateFloaty(ctx, workDir);
                            })
                    );

                return root;
            }
        );

        activity.OnBackPressedCallbacks.Add(() => activity.Finish());
        activity.OnFinishedCallbacks.Add(() =>
        {
            subFloaty?.Dispose();
            menuFloaty?.Dispose();
            tcs.SetResult(true);
        });

        return await tcs.Task;
    }

    public static void CreateFloaty(Context ctx, string workDir)
    {
        var assetsDir = Path.Combine(workDir, "assets");
        var root = new FrameLayout(ctx)
            .Name("menuRoot")
            .BackgroundColor(Colors.Transparent)
            .Children(
                new ImageButton(ctx)
                    .Source(Path.Combine(assetsDir, "robot.png"))
                    .Width(48)
                    .Height(48)
                    .Radius(24)
                    .BackgroundColor(Colors.YellowGreen)
                    .OnTouch(OnMenuTouchListener)
                    .Invoke(view => view.SetScaleType(ScaleType.FitCenter))
            );

        menuFloaty = new FloatyWindow(
            root,
            0,
            200,
            flags: WindowManagerFlags.NotFocusable
                | WindowManagerFlags.LayoutInScreen
                | WindowManagerFlags.LayoutNoLimits
        );

        CreateSubFloaty(ctx, workDir);
    }

    public static void CreateSubFloaty(Context ctx, string workDir)
    {
        var assetsDir = Path.Combine(workDir, "assets");
        var root = new FrameLayout(ctx)
            .Name("subRoot")
            .BackgroundColor(Colors.Transparent)
            .Visibility(ViewStates.Gone)
            .OnTouch(
                (v, e) =>
                {
                    if (
                        e.Action == MotionEventActions.Outside
                        && activity["subRoot"].Visibility == ViewStates.Visible
                    )
                    {
                        SwitchSubFloatyState();
                    }
                }
            )
            .Children(
                new ImageButton(ctx)
                    .Name("subMenu")
                    .Source(Path.Combine(assetsDir, "robot.png"))
                    .Width(48)
                    .Height(48)
                    .Radius(24)
                    .BackgroundColor(Colors.YellowGreen)
                    .OnClick(v => SwitchSubFloatyState())
                    .Invoke(view => view.SetScaleType(ScaleType.FitCenter)),
                new ImageButton(ctx)
                    .Name("sub1")
                    .Source(Path.Combine(assetsDir, "mine.png"))
                    .Width(48)
                    .Height(48)
                    .Radius(24)
                    .BackgroundColor(Colors.YellowGreen)
                    .OnClick(v =>
                    {
                        SwitchSubFloatyState();
                        Globals.Toast("你点击了第1项");
                    })
                    .Invoke(view => view.SetScaleType(ScaleType.FitCenter)),
                new ImageButton(ctx)
                    .Name("sub2")
                    .Source(Path.Combine(assetsDir, "tag.png"))
                    .Width(48)
                    .Height(48)
                    .Radius(24)
                    .BackgroundColor(Colors.YellowGreen)
                    .OnClick(v =>
                    {
                        SwitchSubFloatyState();
                        Globals.Toast("你点击了第2项");
                    })
                    .Invoke(view => view.SetScaleType(ScaleType.FitCenter)),
                new ImageButton(ctx)
                    .Name("sub3")
                    .Source(Path.Combine(assetsDir, "location.png"))
                    .Width(48)
                    .Height(48)
                    .Radius(24)
                    .BackgroundColor(Colors.YellowGreen)
                    .OnClick(v =>
                    {
                        SwitchSubFloatyState();
                        Globals.Toast("你点击了第3项");
                    })
                    .Invoke(view => view.SetScaleType(ScaleType.FitCenter)),
                new ImageButton(ctx)
                    .Name("sub4")
                    .Source(Path.Combine(assetsDir, "discover.png"))
                    .Width(48)
                    .Height(48)
                    .Radius(24)
                    .BackgroundColor(Colors.YellowGreen)
                    .OnClick(v =>
                    {
                        SwitchSubFloatyState();
                        Globals.Toast("你点击了第4项");
                    })
                    .Invoke(view => view.SetScaleType(ScaleType.FitCenter))
            );

        subFloaty = new FloatyWindow(
            root,
            flags: WindowManagerFlags.NotFocusable
                | WindowManagerFlags.LayoutInScreen
                | WindowManagerFlags.LayoutNoLimits
                | WindowManagerFlags.WatchOutsideTouch
        );
    }

    private static void OnMenuTouchListener(ImageButton view, MotionEvent e)
    {
        if (e.Action == MotionEventActions.Down)
        {
            var point = menuFloaty.GetPosition();
            floatyX = point.X;
            floatyY = point.Y;
            downX = e.RawX;
            downY = e.RawY;
        }
        else if (e.Action == MotionEventActions.Move)
        {
            var offsetX = e.RawX - downX;
            var offsetY = e.RawY - downY;
            if (!isMoving)
            {
                if (Math.Abs(offsetX) < 25 && Math.Abs(offsetY) < 25)
                    return;
            }

            isMoving = true;

            menuFloaty.SetPosition(
                (int)view.Context.FromPixels(floatyX + offsetX),
                (int)view.Context.FromPixels(floatyY + offsetY)
            );
        }
        else if (e.Action == MotionEventActions.Up)
        {
            var width = Devices.Width;

            if (isMoving)
            {
                var point = menuFloaty.GetPosition();
                if (point.X < width / 2)
                    menuFloaty.SetPosition(0, (int)view.Context.FromPixels(point.Y));
                else
                    menuFloaty.SetPosition(
                        (int)view.Context.FromPixels(width - activity["menuRoot"].Width),
                        (int)view.Context.FromPixels(point.Y)
                    );

                isMoving = false;
            }
            else
            {
                SwitchSubFloatyState();
            }
        }
    }

    private static void SwitchSubFloatyState()
    {
        var width = Devices.Width;
        var height = Devices.Height;

        var menuPoint = menuFloaty.GetPosition();
        var onRight = menuPoint.X > width / 2;
        var gravity = onRight
            ? GravityFlags.Center | GravityFlags.Right
            : GravityFlags.Center | GravityFlags.Left;

        var subBtns = new View[]
        {
            activity["sub1"],
            activity["sub2"],
            activity["sub3"],
            activity["sub4"],
        };

        var subRoot = activity["subRoot"];
        if (subRoot.Visibility != ViewStates.Visible)
        {
            if (subRoot.LayoutParameters is WindowManagerLayoutParams lp)
            {
                lp.X = (int)(menuPoint.X - (onRight ? subRoot.Context.ToPixels(48 * 2.5 - 48) : 0));
                lp.Y = (int)(menuPoint.Y - subRoot.Context.ToPixels(48 * 1.25));
                lp.Width = (int)subRoot.Context.ToPixels(48 * 2.5);
                lp.Height = (int)subRoot.Context.ToPixels(48 * 3.5);

                var windowManager = subRoot
                    .Context.GetSystemService("window")
                    .JavaCast<IWindowManager>();
                windowManager.UpdateViewLayout(subRoot, lp);
            }

            subRoot.Visibility = ViewStates.Visible;

            activity["subMenu"].LayoutGravity(gravity);

            var animatorSet = new AnimatorSet();
            for (int i = 0; i < subBtns.Length; i++)
            {
                View subBtn = subBtns[i];
                var ctx = subBtn.Context;

                subBtn.LayoutGravity(gravity);

                var degree = 180 / (subBtns.Length - 1) * (subBtns.Length - i - 1) * Math.PI / 180;
                var offsetX = ctx.ToPixels(48 * 1.25) * Math.Sin(degree) + ctx.ToPixels(48 / 3);
                var offsetY = ctx.ToPixels(48 * 1.25) * Math.Cos(degree);
                var transX = ObjectAnimator.OfFloat(
                    subBtn,
                    "translationX",
                    0,
                    onRight ? -(float)offsetX : (float)offsetX
                );
                var transY = ObjectAnimator.OfFloat(subBtn, "translationY", 0, (float)offsetY);
                var scaleX = ObjectAnimator.OfFloat(subBtn, "scaleX", 0, 1);
                var scaleY = ObjectAnimator.OfFloat(subBtn, "scaleY", 0, 1);
                animatorSet.Play(transX).With(transY).With(scaleX).With(scaleY);
            }

            animatorSet.SetDuration(250);
            animatorSet.Start();
        }
        else
        {
            var animatorSet = new AnimatorSet();
            for (int i = 0; i < subBtns.Length; i++)
            {
                View subBtn = subBtns[i];
                var ctx = subBtn.Context;
                var degree = 180 / (subBtns.Length - 1) * (subBtns.Length - i - 1) * Math.PI / 180;
                var offsetX = ctx.ToPixels(48) * 1.25 * Math.Sin(degree) + ctx.ToPixels(48) / 3;
                var offsetY = ctx.ToPixels(48) * 1.25 * Math.Cos(degree);

                var transX = ObjectAnimator.OfFloat(
                    subBtn,
                    "translationX",
                    onRight ? -(float)offsetX : (float)offsetX,
                    0
                );
                var transY = ObjectAnimator.OfFloat(subBtn, "translationY", (float)offsetY, 0);
                var scaleX = ObjectAnimator.OfFloat(subBtn, "scaleX", 1, 0);
                var scaleY = ObjectAnimator.OfFloat(subBtn, "scaleY", 1, 0);
                animatorSet.Play(transX).With(transY).With(scaleX).With(scaleY);
            }
            animatorSet.SetDuration(250);

            animatorSet.AnimationEnd += (s, e) =>
            {
                subRoot.Visibility = ViewStates.Gone;
            };

            animatorSet.Start();
        }
    }
}
