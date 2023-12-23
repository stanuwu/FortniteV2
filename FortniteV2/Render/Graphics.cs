using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Threading;
using FortniteV2.Features;
using FortniteV2.Game;
using FortniteV2.Utils;
using SharpDX;
using SharpDX.Direct3D9;
using FontWeight = SharpDX.Direct3D9.FontWeight;

namespace FortniteV2.Render
{
    public class Graphics : TickThread
    {
        private static readonly VertexElement[] VertexElements =
        {
            new VertexElement(0, 0, DeclarationType.Float4, DeclarationMethod.Default, DeclarationUsage.PositionTransformed, 0),
            new VertexElement(0, 16, DeclarationType.Color, DeclarationMethod.Default, DeclarationUsage.Color, 0),
            VertexElement.VertexDeclarationEnd
        };

        public Graphics(GameProcess gameProcess, GameData gameData, WindowOverlay windowOverlay) : base(nameof(Graphics))
        {
            WindowOverlay = windowOverlay;
            OldRes = new Vector2(WindowOverlay.Window.Width, WindowOverlay.Window.Height);
            GameProcess = gameProcess;
            GameData = gameData;
            FpsCounter = new FpsCounter();

            InitDevice();
        }

        private WindowOverlay WindowOverlay { get; set; }
        private Vector2 OldRes { get; set; }
        public GameProcess GameProcess { get; private set; }
        public GameData GameData { get; private set; }
        public Device Device { get; set; }
        public Font FontAzonix64 { get; private set; }
        public Font FontConsolas32 { get; private set; }
        public List<Vertex> Vertices { get; } = new List<Vertex>();
        public FpsCounter FpsCounter { get; private set; } = new FpsCounter();

        public override void Dispose()
        {
            base.Dispose();

            FontAzonix64.Dispose();
            FontAzonix64 = default;
            FontConsolas32.Dispose();
            FontConsolas32 = default;
            Device.Dispose();
            Device = default;

            FpsCounter = default;
            GameData = default;
            GameProcess = default;
            WindowOverlay = default;
        }

        private void InitDevice()
        {
            var parameters = new PresentParameters
            {
                Windowed = true,
                SwapEffect = SwapEffect.Discard,
                DeviceWindowHandle = WindowOverlay.Window.Handle,
                MultiSampleQuality = 0,
                BackBufferFormat = Format.A8R8G8B8,
                BackBufferWidth = WindowOverlay.Window.Width,
                BackBufferHeight = WindowOverlay.Window.Height,
                EnableAutoDepthStencil = true,
                AutoDepthStencilFormat = Format.D16,
                PresentationInterval = PresentInterval.Immediate,
                MultiSampleType = MultisampleType.TwoSamples
            };

            Device = new Device(new Direct3D(), 0, DeviceType.Hardware, WindowOverlay.Window.Handle, CreateFlags.HardwareVertexProcessing, parameters);

            var azonix64 = new FontDescription
            {
                Height = 64,
                Italic = false,
                CharacterSet = FontCharacterSet.Ansi,
                FaceName = "Azonix",
                MipLevels = 0,
                OutputPrecision = FontPrecision.TrueType,
                PitchAndFamily = FontPitchAndFamily.Default,
                Quality = FontQuality.ClearType,
                Weight = FontWeight.Regular
            };
            FontAzonix64 = new Font(Device, azonix64);

            var consolas32 = new FontDescription
            {
                Height = 32,
                Italic = false,
                CharacterSet = FontCharacterSet.Ansi,
                FaceName = "Consolas",
                MipLevels = 0,
                OutputPrecision = FontPrecision.TrueType,
                PitchAndFamily = FontPitchAndFamily.Default,
                Quality = FontQuality.ClearType,
                Weight = FontWeight.Regular
            };
            FontConsolas32 = new Font(Device, consolas32);
        }

        protected override void Tick()
        {
            if (!GameProcess.IsValid) return;

            var newRes = new Vector2(WindowOverlay.Window.Width, WindowOverlay.Window.Height);
            if (!Equals(OldRes, newRes))
            {
                Device.Dispose();
                FontAzonix64.Dispose();
                FontConsolas32.Dispose();
                InitDevice();
            }

            OldRes = newRes;

            FpsCounter.Update();

            Application.Current.Dispatcher.Invoke(() =>
            {
                Device.SetRenderState(RenderState.AlphaBlendEnable, true);
                Device.SetRenderState(RenderState.AlphaTestEnable, false);
                Device.SetRenderState(RenderState.SourceBlend, Blend.SourceAlpha);
                Device.SetRenderState(RenderState.DestinationBlend, Blend.InverseSourceAlpha);
                Device.SetRenderState(RenderState.Lighting, false);
                Device.SetRenderState(RenderState.CullMode, Cull.None);
                Device.SetRenderState(RenderState.ZEnable, true);
                Device.SetRenderState(RenderState.ZFunc, Compare.Always);

                Device.Clear(ClearFlags.Target | ClearFlags.ZBuffer, Color.FromAbgr(0), 1, 0);

                Device.BeginScene();
                Render();
                Device.EndScene();

                Device.Present();
            }, DispatcherPriority.Render);
        }

        private void Render()
        {
            Vertices.Clear();

            Draw();

            var count = Vertices.Count;
            var vertices = new VertexBuffer(Device, count * 20, Usage.WriteOnly, VertexFormat.None, Pool.Managed);
            vertices.Lock(0, 0, LockFlags.None)
                .WriteRange(Vertices.ToArray());
            vertices.Unlock();

            Device.SetStreamSource(0, vertices, 0, 20);
            var vertexDecl = new VertexDeclaration(Device, VertexElements);
            Device.VertexDeclaration = vertexDecl;
            Device.DrawPrimitives(PrimitiveType.LineList, 0, count / 2);

            vertices.Dispose();
            vertexDecl.Dispose();
        }

        private void Draw()
        {
            // draw here
            SkeletonEsp.Draw(this);
            RecoilCrosshair.Draw(this);
            Hud.Draw(GameProcess, this);
        }

        public void DrawLineWorld(Color color, params Vector3[] verticesWorld)
        {
            var verticesScreen = verticesWorld
                .Select(v => GameData.Player.MatrixViewProjectionViewport.Transform(v))
                .Where(v => v.Z < 1)
                .Select(v => new Vector2(v.X, v.Y)).ToArray();
            DrawLine(color, verticesScreen);
        }

        public void DrawLine(Color color, params Vector2[] verts)
        {
            if (verts.Length < 2 || verts.Length % 2 != 0) return;

            foreach (var vertex in verts) Vertices.Add(new Vertex { Color = color, Position = new Vector4(vertex.X, vertex.Y, 0.5f, 1.0f) });
        }
    }
}