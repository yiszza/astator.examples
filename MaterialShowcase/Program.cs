using System.Threading.Tasks;
using Android.Content;
using Android.Content.Res;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.Util;
using Android.Views;
using Android.Widget;
using AndroidX.Core.Content;
using AndroidX.Core.View;
using AndroidX.DrawerLayout.Widget;
using AndroidX.ViewPager2.Widget;
using Astator.Platform.Droid.UI;
using Astator.Platform.Droid.UI.Base;
using Astator.Script;
using Google.Android.Material.AppBar;
using Google.Android.Material.BottomNavigation;
using Google.Android.Material.Navigation;
using static Android.Views.ViewGroup;
using Orientation = Android.Widget.Orientation;
using Path = System.IO.Path;

namespace MaterialShowcase;

public class Program
{
    private static readonly TaskCompletionSource<bool> tcs = new();

    [EntryMethod]
    public static async Task<bool> MainAsync(string workDir)
    {
        var activity = await TemplateActivity.CreateAsync();
        activity.SetContentViewDelegate(workDir, CreateRoot);

        return await tcs.Task;
    }

    internal static View CreateRoot(Context ctx, string workDir)
    {
        var activity = (TemplateActivity)ctx;

        activity.OnBackPressedCallbacks.Add(activity.Finish);
        activity.OnFinishedCallbacks.Add(() => tcs.TrySetResult(true));

        var assetsDir = Path.Combine(workDir, "assets");

        using var tv = new TypedValue();
        ctx.Theme?.ResolveAttribute(Resource.Attribute.colorOnSurface, tv, true);
        var onSurfaceTint = ContextCompat.GetColorStateList(ctx, tv.ResourceId);

        ctx.Theme?.ResolveAttribute(Resource.Attribute.colorSurface, tv, true);
        var backgroundColor = ContextCompat.GetColor(ctx, tv.ResourceId);

        activity.SetStatusBarColor(
            new Color(backgroundColor),
            (ctx.Resources!.Configuration!.UiMode & UiMode.NightMask) == UiMode.NightYes
        );

        var root = new DrawerLayout(ctx)
            .Name("drawer")
            .Children(
                new LinearLayout(ctx)
                    .Orientation(Orientation.Vertical)
                    .LayoutParams(
                        new LayoutParams(LayoutParams.MatchParent, LayoutParams.MatchParent)
                    )
                    .Children(
                        new AppBarLayout(ctx).Children(
                            new MaterialToolbar(ctx)
                                .Name("topBar")
                                .Invoke(view =>
                                {
                                    view.Title = "Home";
                                    view.TitleCentered = false;
                                    view.NavigationIcon = SvgDrawable.FromSvg(
                                        ctx.Resources,
                                        Path.Combine(assetsDir, "menu.svg")
                                    );
                                    view.NavigationClick += (s, e) =>
                                    {
                                        if (activity["drawer"] is DrawerLayout drawer)
                                        {
                                            drawer.OpenDrawer(gravity: GravityCompat.Start);
                                        }
                                    };

                                    view.SetNavigationIconTint(onSurfaceTint!.DefaultColor);

                                    view.Menu?.Add(0, 0, 0, "link")!
                                        .SetIcon(
                                            SvgDrawable.FromSvg(
                                                ctx.Resources,
                                                Path.Combine(assetsDir, "link.svg")
                                            )
                                        )
                                        .SetIconTintList(onSurfaceTint)
                                        .SetShowAsAction(ShowAsAction.IfRoom);

                                    view.Menu?.Add(0, 1, 1, "calendar")!
                                        .SetIcon(
                                            SvgDrawable.FromSvg(
                                                ctx.Resources,
                                                Path.Combine(assetsDir, "calendar.svg")
                                            )
                                        )
                                        .SetIconTintList(onSurfaceTint)
                                        .SetShowAsAction(ShowAsAction.IfRoom);

                                    view.Menu?.Add(1, 0, 0, "menu")!
                                        .SetIcon(
                                            SvgDrawable.FromSvg(
                                                ctx.Resources,
                                                Path.Combine(assetsDir, "menu.svg")
                                            )
                                        )
                                        .SetIconTintList(onSurfaceTint)
                                        .SetShowAsAction(ShowAsAction.Never);
                                })
                        ),
                        new ViewPager2(ctx)
                            .Name("viewPager")
                            .LayoutParams(
                                new LinearLayout.LayoutParams(LayoutParams.MatchParent, 0, 1)
                            )
                            .Adapter(
                                new ViewPagerAdapter(
                                    new FrameLayout(ctx)
                                        .BackgroundColor(backgroundColor)
                                        .LayoutParams(
                                            new LayoutParams(
                                                LayoutParams.MatchParent,
                                                LayoutParams.MatchParent
                                            )
                                        )
                                        .Children(
                                            new TextView(ctx)
                                                .Text("Home")
                                                .TextSize(20)
                                                .Gravity(GravityFlags.Center)
                                        ),
                                    new FrameLayout(ctx)
                                        .BackgroundColor(backgroundColor)
                                        .LayoutParams(
                                            new LayoutParams(
                                                LayoutParams.MatchParent,
                                                LayoutParams.MatchParent
                                            )
                                        )
                                        .Children(
                                            new TextView(ctx)
                                                .Text("Music")
                                                .TextSize(20)
                                                .Gravity(GravityFlags.Center)
                                        ),
                                    new FrameLayout(ctx)
                                        .BackgroundColor(backgroundColor)
                                        .LayoutParams(
                                            new LayoutParams(
                                                LayoutParams.MatchParent,
                                                LayoutParams.MatchParent
                                            )
                                        )
                                        .Children(
                                            new TextView(ctx)
                                                .Text("Podcasts")
                                                .TextSize(20)
                                                .Gravity(GravityFlags.Center)
                                        ),
                                    new FrameLayout(ctx)
                                        .BackgroundColor(backgroundColor)
                                        .LayoutParams(
                                            new LayoutParams(
                                                LayoutParams.MatchParent,
                                                LayoutParams.MatchParent
                                            )
                                        )
                                        .Children(
                                            new TextView(ctx)
                                                .Text("Settings")
                                                .TextSize(20)
                                                .Gravity(GravityFlags.Center)
                                        )
                                )
                            )
                            .Invoke(view =>
                            {
                                view.RegisterOnPageChangeCallback(
                                    new ViewPagerOnPageChangeCallback(position =>
                                    {
                                        if (activity["navBar"] is BottomNavigationView navBar)
                                        {
                                            if (navBar.SelectedItemId != position)
                                                navBar.SelectedItemId = position;
                                        }
                                    })
                                );
                            }),
                        new BottomNavigationView(ctx)
                            .Name("navBar")
                            .Invoke(view =>
                            {
                                view.Menu.Add(0, 0, 0, "Home")!
                                    .SetIcon(
                                        new StateListDrawable()
                                            .AddState(
                                                SvgDrawable.FromSvg(
                                                    ctx.Resources,
                                                    Path.Combine(workDir, "assets", "home_fill.svg")
                                                ),
                                                Android.Resource.Attribute.StateChecked
                                            )
                                            .AddState(
                                                SvgDrawable.FromSvg(
                                                    ctx.Resources,
                                                    Path.Combine(assetsDir, "home.svg")
                                                )
                                            )
                                    );
                                view.Menu.Add(0, 1, 1, "Music")!
                                    .SetIcon(
                                        SvgDrawable.FromSvg(
                                            ctx.Resources,
                                            Path.Combine(assetsDir, "music.svg")
                                        )
                                    );
                                view.Menu.Add(0, 2, 2, "Podcasts")!
                                    .SetIcon(
                                        SvgDrawable.FromSvg(
                                            ctx.Resources,
                                            Path.Combine(assetsDir, "podcasts.svg")
                                        )
                                    );
                                view.Menu.Add(0, 3, 3, "Settings")!
                                    .SetIcon(
                                        new StateListDrawable()
                                            .AddState(
                                                SvgDrawable.FromSvg(
                                                    ctx.Resources,
                                                    Path.Combine(
                                                        workDir,
                                                        "assets",
                                                        "settings_fill.svg"
                                                    )
                                                ),
                                                Android.Resource.Attribute.StateChecked
                                            )
                                            .AddState(
                                                SvgDrawable.FromSvg(
                                                    ctx.Resources,
                                                    Path.Combine(assetsDir, "settings.svg")
                                                )
                                            )
                                    );

                                view.ItemSelected += (sender, e) =>
                                {
                                    if (
                                        activity["viewPager"] is ViewPager2 vp
                                        && vp.CurrentItem != e.Item.ItemId
                                    )
                                    {
                                        vp.CurrentItem = e.Item.ItemId;
                                    }

                                    if (activity["topBar"] is MaterialToolbar topBar)
                                    {
                                        topBar.Title =
                                            (activity["navBar"] as BottomNavigationView)
                                                ?.Menu?.FindItem(e.Item.ItemId)
                                                ?.TitleFormatted?.ToString() ?? "Home";
                                    }

                                    e.Handled = true;
                                };
                            })
                    ),
                new NavigationView(ctx)
                    .LayoutParams(
                        new DrawerLayout.LayoutParams(
                            LayoutParams.WrapContent,
                            LayoutParams.MatchParent,
                            GravityCompat.Start
                        )
                    )
                    .Invoke(view =>
                    {
                        view.AddHeaderView(
                            new TextView(ctx)
                                .Text("Mail")
                                .TextSize(16f)
                                .Padding(32, 12, 32, 12)
                                .Gravity(GravityFlags.CenterVertical)
                        );

                        view.Menu.Add(0, 0, 0, "Inbox")!
                            .SetIcon(
                                SvgDrawable.FromSvg(
                                    ctx.Resources,
                                    Path.Combine(assetsDir, "inbox.svg")
                                )
                            );
                        view.Menu.Add(0, 1, 1, "Outbox")!
                            .SetIcon(
                                SvgDrawable.FromSvg(
                                    ctx.Resources,
                                    Path.Combine(assetsDir, "outbox.svg")
                                )
                            );
                        view.Menu.Add(0, 2, 2, "Favorites")!
                            .SetIcon(
                                SvgDrawable.FromSvg(
                                    ctx.Resources,
                                    Path.Combine(assetsDir, "favorites.svg")
                                )
                            );
                        view.Menu.Add(0, 3, 3, "Trash")!
                            .SetIcon(
                                SvgDrawable.FromSvg(
                                    ctx.Resources,
                                    Path.Combine(assetsDir, "trash.svg")
                                )
                            );
                        view.Menu.Add(1, 0, 4, "Label")!
                            .SetIcon(
                                SvgDrawable.FromSvg(
                                    ctx.Resources,
                                    Path.Combine(assetsDir, "label.svg")
                                )
                            );
                    })
            );

        return root;
    }
}
