using System;
using System.IO;
using System.Threading.Tasks;
using Android.Graphics.Drawables;
using Android.Text.Method;
using Android.Views;
using Android.Widget;
using Astator.Platform.Droid.UI;
using Astator.Platform.Droid.UI.Base;
using Astator.Script;
using Microsoft.Maui.Graphics;
using Microsoft.Maui.Platform;
using static Android.Views.ViewGroup;
using static Android.Widget.ImageView;
using Orientation = Android.Widget.Orientation;

namespace UIShowcase;

public class Program
{
    private static readonly TaskCompletionSource<bool> tcs = new();

    [EntryMethod]
    public static async Task<bool> MainAsync(string workDir)
    {
        var activity = await TemplateActivity.CreateAsync();

        activity.OnBackPressedCallbacks.Add(activity.Finish);
        activity.OnFinishedCallbacks.Add(() => tcs.TrySetResult(true));

        activity.SetContentViewDelegate(
            workDir,
            (ctx, workDir) =>
            {
                try
                {
                    var root = new LinearLayout(ctx)
                        .Orientation(Orientation.Vertical)
                        .LayoutParams(
                            new LayoutParams(LayoutParams.MatchParent, LayoutParams.MatchParent)
                        )
                        .Children(
                            new TextView(ctx)
                                .Text("控件展示")
                                .Padding(12, 12, 12, 12)
                                .TextStyle(Android.Graphics.TypefaceStyle.Bold)
                                .TextSize(20)
                                .Gravity(GravityFlags.Left),
                            new ScrollView(ctx).Children(
                                new LinearLayout(ctx)
                                    .Orientation(Orientation.Vertical)
                                    .Padding(20, 0, 20, 0)
                                    .Invoke(v =>
                                    {
                                        v.ShowDividers = ShowDividers.Middle;
                                        var spacer = new GradientDrawable();
                                        spacer.SetSize(0, (int)ctx.ToPixels(12));
                                        v.SetDividerDrawable(spacer);
                                    })
                                    .LayoutParams(
                                        new LayoutParams(
                                            LayoutParams.MatchParent,
                                            LayoutParams.MatchParent
                                        )
                                    )
                                    .Children(
                                        new TextView(ctx).Text("文本").TextSize(16),
                                        new TextView(ctx)
                                            .Text("加粗文本")
                                            .TextSize(16)
                                            .TextStyle(Android.Graphics.TypefaceStyle.Bold),
                                        new TextView(ctx)
                                            .Text("加粗斜体文本")
                                            .TextSize(16)
                                            .TextStyle(Android.Graphics.TypefaceStyle.BoldItalic),
                                        new TextView(ctx)
                                            .Text("超链接文本: https://www.bing.com")
                                            .TextSize(16)
                                            .AutoLinkMask(Android.Text.Util.MatchOptions.WebUrls)
                                            .MovementMethod(LinkMovementMethod.Instance),
                                        new EditText(ctx).Hint("编辑框提示文本").TextSize(16),
                                        new Button(ctx).Text("按钮").TextSize(16),
                                        new Button(ctx)
                                            .Text("圆角按钮")
                                            .BackgroundColor(Colors.YellowGreen)
                                            .TextSize(16)
                                            .Radius(18),
                                        new Button(ctx)
                                            .Text("点我")
                                            .TextSize(16)
                                            .OnClick(v => v.Text += "."),
                                        new ImageButton(ctx)
                                            .Source(Path.Combine(workDir, "assets", "mine.png"))
                                            .BackgroundColor(Colors.YellowGreen)
                                            .Width(48)
                                            .Height(48)
                                            .Invoke(view => view.SetScaleType(ScaleType.FitCenter)),
                                        //圆角
                                        new ImageButton(ctx)
                                            .Source(Path.Combine(workDir, "assets", "mine.png"))
                                            .BackgroundColor(Colors.YellowGreen)
                                            .Width(48)
                                            .Height(48)
                                            .Radius(24)
                                            .Invoke(view => view.SetScaleType(ScaleType.FitCenter)),
                                        //使用svg
                                        new ImageButton(ctx)
                                            .SvgSource(
                                                Path.Combine(workDir, "assets", "home.svg"),
                                                36,
                                                36
                                            )
                                            .BackgroundColor(Colors.YellowGreen)
                                            .Width(48)
                                            .Height(48),
                                        new Switch(ctx).Text("开关"),
                                        new RadioGroup(ctx)
                                            .Orientation(Orientation.Horizontal)
                                            .Children(
                                                new RadioButton(ctx).Text("选项1"),
                                                new RadioButton(ctx).Text("选项2"),
                                                new RadioButton(ctx).Text("选项3")
                                            )
                                            .CheckAt(1),
                                        new RadioGroup(ctx)
                                            .Orientation(Orientation.Vertical)
                                            .Children(
                                                new RadioButton(ctx).Text("选项1"),
                                                new RadioButton(ctx).Text("选项2"),
                                                new RadioButton(ctx).Text("选项3")
                                            )
                                            .CheckAt(2),
                                        new Spinner(ctx)
                                            .Items("选项1", "选项2", "选项3")
                                            .Position(2)
                                    )
                            )
                        );
                    return root;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
                return default;
            }
        );

        return await tcs.Task;
    }
}
