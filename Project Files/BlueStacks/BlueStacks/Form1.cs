using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.Xml;
using Tesseract;
using Timer = System.Timers.Timer;
using C = System.Convert;

namespace BlueStacks {
	public partial class Form1 : Form {
		private string x1_add;
		private string y1_add;
		private string len;
		private string height;
		private string next_x;
		private string next_y;
		const string defaultContents =
@"<?xml version=""1.0"" encoding=""utf-8""?>
<config>
	<setting key=""x1_add"" value=""58"" />
	<setting key=""y1_add"" value=""82"" />
	<setting key=""len"" value=""115"" />
	<setting key=""height"" value=""70"" />
	<setting key=""next_x"" value=""150"" />
	<setting key=""next_y"" value=""300"" />
</config>";
		public Form1() {
			InitializeComponent();
			if ( !File.Exists( "./settings.xml" ) ) {
				using ( var sw = new StreamWriter( "./settings.xml" ) ) {
					sw.Write( defaultContents );
				}
			}
			Settings.Initialize();
			textBox5.Text = x1_add = Settings.Get( "x1_add" );
			textBox6.Text = y1_add = Settings.Get( "y1_add" );
			textBox8.Text = len = Settings.Get( "len" );
			textBox7.Text = height = Settings.Get( "height" );
			textBox10.Text = next_x = Settings.Get( "next_x" );
			textBox9.Text = next_y = Settings.Get( "next_y" );
			var t = new Thread( () => {
				var t1 = new Timer( 500 );
				t1.Elapsed += delegate {
					if ( Util.GetActiveWindowTitle() == "BlueStacks App Player" ) {
						Util._RECT r;
						Util.GetWindowRect( Util.GetActiveWindow(), out r );
						int x1 = r.Left + 8;
						int y1 = r.Top + 30;
						var rect = new Rectangle( x1 + C.ToInt32( x1_add ), y1 + C.ToInt32( y1_add ), C.ToInt32( len ), C.ToInt32( height ) );
						Bitmap b = new Bitmap( rect.Width, rect.Height );
						Graphics g = Graphics.FromImage( b );
						try {
							g.CopyFromScreen( rect.Left, rect.Top, 0, 0, b.Size, CopyPixelOperation.SourceCopy );
						} catch {
						}
						try {
							for ( int i = 0; i <= rect.Width; ++i ) {
								for ( int ii = 0; ii <= rect.Height; ++ii ) {
									if ( i < rect.Width && ii < rect.Height ) {
										Color c = b.GetPixel( i, ii );
										if ( c != Util.Gold && c != Util.Elixir ) {
											b.SetPixel( i, ii, Color.Black );
										} else {
											b.SetPixel( i, ii, Color.White );
										}
									}
								}
							}
						} catch {
						}
						try {
							pictureBox1.Image = b;
						} catch {
						}
						try {
							using ( var eng = new TesseractEngine( Directory.GetCurrentDirectory() + @".\tessdata", "eng", EngineMode.Default ) ) {
								eng.SetVariable( "tessedit_char_whitelist", "1234567890" );
								label6.Invoke( new Action( delegate {
									label6.Text = String.Empty;
								} ) );
								try {
									using ( var img = PixConverter.ToPix( b ) ) {
										using ( var txt = eng.Process( img ) ) {
											var text = txt.GetText();
											string[] spl = text.Split( '\n' );
											textBox1.Invoke( new Action( delegate {
												try {
													textBox1.Text = spl[ 0 ].Replace( " ", String.Empty );
													textBox2.Text = spl[ 1 ].Replace( " ", String.Empty );
												} catch ( IndexOutOfRangeException ) {
												}
												int minG = 0;
												int minE = 0;
												int curG = 0;
												int curE = 0;
												try {
													minG = Convert.ToInt32( textBox3.Text );
												} catch {
												}
												try {
													minE = Convert.ToInt32( textBox4.Text );
												} catch {
												}
												try {
													curG = Convert.ToInt32( spl[ 0 ].Replace( " ", String.Empty ) );
												} catch {
												}
												try {
													curE = Convert.ToInt32( spl[ 1 ].Replace( " ", String.Empty ) );
												} catch {
												}
												if ( curG >= minG || curE >= minE ) {
													checkBox1.Invoke( new Action( delegate {
														checkBox1.Checked = false;
													} ) );
												} else {
													if ( checkBox1.Checked ) {
														if ( ( textBox3.Text.Length > 0 || textBox4.Text.Length > 0 ) && ( curE != 0 || curG != 0 ) ) {
															Debug.WriteLine( r.Bottom );
															Debug.WriteLine( C.ToInt32( next_y ) );
															Util.LeftMouseClick( r.Right - C.ToInt32( next_x ), r.Bottom - C.ToInt32( next_y ) );
														}
													}
												}
											} ) );
										}
									}
								} catch {
								}
							}
						} catch ( TesseractException ) {
							label6.Invoke( new Action( delegate {
								label6.Text = "Cannot Initialize Tesseract Engine";
							} ) );
						}
						g.Dispose();
					}
				};
				t1.Start();
			} );
			t.Start();
		}

		private void textBox3_KeyPress( object sender, KeyPressEventArgs e ) {
			if ( !char.IsControl( e.KeyChar ) && !char.IsDigit( e.KeyChar ) && ( e.KeyChar != '.' ) ) {
				e.Handled = true;
			}
		}

		private void textBox4_KeyPress( object sender, KeyPressEventArgs e ) {
			if ( !char.IsControl( e.KeyChar ) && !char.IsDigit( e.KeyChar ) && ( e.KeyChar != '.' ) ) {
				e.Handled = true;
			}
		}

		private void textBox5_TextChanged( object sender, EventArgs e ) {
			x1_add = textBox5.Text;
		}

		private void textBox6_TextChanged( object sender, EventArgs e ) {
			y1_add = textBox6.Text;
		}

		private void textBox8_TextChanged( object sender, EventArgs e ) {
			len = textBox8.Text;
		}

		private void textBox7_TextChanged( object sender, EventArgs e ) {
			height = textBox7.Text;
		}

		private void textBox10_TextChanged( object sender, EventArgs e ) {
			next_x = textBox10.Text;
		}

		private void textBox9_TextChanged( object sender, EventArgs e ) {
			next_y = textBox9.Text;
		}

		private void button1_Click( object sender, EventArgs e ) {
			Settings.Set( "x1_add", x1_add );
			Settings.Set( "y1_add", y1_add );
			Settings.Set( "len", len );
			Settings.Set( "height", height );
			Settings.Set( "next_x", next_x );
			Settings.Set( "next_y", next_y );
		}
	}

	public class Util {
		public static Color Gold = Color.FromArgb( 255, 255, 251, 204 );
		public static Color Elixir = Color.FromArgb( 255, 255, 232, 253 );

		[DllImport( "user32.dll" )]
		private static extern bool SetCursorPos( int x, int y );

		[DllImport( "user32.dll" )]
		public static extern void mouse_event( int dwFlags, int dx, int dy, int cButtons, int dwExtraInfo );

		public const int MOUSEEVENTF_LEFTDOWN = 0x02;
		public const int MOUSEEVENTF_LEFTUP = 0x04;

		public static void LeftMouseClick( int xpos, int ypos ) {
			SetCursorPos( xpos, ypos );
			mouse_event( MOUSEEVENTF_LEFTDOWN, xpos, ypos, 0, 0 );
			mouse_event( MOUSEEVENTF_LEFTUP, xpos, ypos, 0, 0 );
		}

		[DllImport( "user32.dll" )]
		private static extern IntPtr GetDC( IntPtr hwnd );

		[DllImport( "user32.dll" )]
		private static extern Int32 ReleaseDC( IntPtr hwnd, IntPtr hdc );

		[DllImport( "gdi32.dll" )]
		private static extern uint GetPixel( IntPtr hdc, int nXPos, int nYPos );

		[DllImport( "user32.dll" )]
		private static extern IntPtr GetForegroundWindow();

		[DllImport( "user32.dll" )]
		private static extern int GetWindowText( IntPtr hWnd, StringBuilder text, int count );

		[DllImport( "user32.dll" )]
		[return: MarshalAs( UnmanagedType.Bool )]
		public static extern bool GetWindowRect( IntPtr hWnd, out _RECT lpRect );

		public static IntPtr GetActiveWindow() {
			return GetForegroundWindow();
		}

		[StructLayout( LayoutKind.Sequential )]
		public struct _RECT {
			public int Left;
			public int Top;
			public int Right;
			public int Bottom;
		}

		public static Color GetPixelColor( int x, int y ) {
			IntPtr hdc = GetDC( IntPtr.Zero );
			uint pixel = GetPixel( hdc, x, y );
			ReleaseDC( IntPtr.Zero, hdc );
			int one = 0x000000FF;
			int two = 0x0000FF00;
			int three = 0x00FF0000;
			Color color = Color.FromArgb( ( int ) ( pixel & one ), ( int ) ( pixel & two ) >> 8, ( int ) ( pixel & three ) >> 16 );
			return color;
		}

		public static string GetActiveWindowTitle() {
			const int nChars = 256;
			StringBuilder Buff = new StringBuilder( nChars );
			IntPtr handle = GetForegroundWindow();

			if ( GetWindowText( handle, Buff, nChars ) > 0 ) {
				return Buff.ToString();
			}
			return null;
		}
	}
	public static class Settings {
		private static XmlDocument doc;
		private const string settingsfile = "./settings.xml";
		public static void Initialize() {
			doc = new XmlDocument();
			doc.Load( settingsfile );
		}
		public static string Get( string key, string def = "" ) {
			XmlNode node = doc.SelectSingleNode( "config/setting[@key='" + key + "']" );
			if ( node == null ) {
				return def;
			}
			return node.Attributes[ "value" ].Value ?? def;
		}
		public static void Set( string key, string value ) {
			XmlNode node = doc.SelectSingleNode( "config/setting[@key='" + key + "']" );
			if ( node != null ) {
				node.Attributes[ "value" ].Value = value;
				doc.Save( settingsfile );
			}
		}
	}
}
