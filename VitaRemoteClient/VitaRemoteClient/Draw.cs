using System;
using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Environment;
using Sce.PlayStation.Core.Graphics;
using Sce.PlayStation.Core.Imaging;

namespace VitaRemoteClient
{
	public static class Draw
	{
		private static GraphicsContext graphics;
		private static ShaderProgram shaderProgram;
		private static  float[] vertices=new float[12];
		
		private static float[] texcoords = {
			0.0f, 0.0f,	// 0 top left.
			0.0f, 1.0f,	// 1 bottom left.
			1.0f, 0.0f,	// 2 top right.
			1.0f, 1.0f,	// 3 bottom right.
		};
		
		private static float[] colors = {
			1.0f,	1.0f,	1.0f,	1.0f,	// 0 top left.
			1.0f,	1.0f,	1.0f,	1.0f,	// 1 bottom left.
			1.0f,	1.0f,	1.0f,	1.0f,	// 2 top right.
			1.0f,	1.0f,	1.0f,	1.0f,	// 3 bottom right.
		};
		
		private  const int indexSize = 4;
		private static ushort[] indices;

		private static VertexBuffer vertexBuffer;
		
		private static float Width;
		
		private static float Height;
		
		private static Matrix4 unitScreenMatrix;
		
		public static void init(GraphicsContext graphicContext)
		{
			graphics = graphicContext;
			shaderProgram = new ShaderProgram("/Application/shaders/Simple.cgx");
			shaderProgram.SetUniformBinding(0, "u_WorldMatrix");
			ImageRect rectScreen = graphics.Screen.Rectangle;
			Width = 940;//jonathan	orignal value 940
			Height = 544;//jonathan originla value 544

			vertices[0]=0.0f;	// x0
			vertices[1]=0.0f;	// y0
			vertices[2]=0.0f;	// z0
			
			vertices[3]=0.0f;	// x1
			vertices[4]=1.0f;	// y1
			vertices[5]=0.0f;	// z1
			
			vertices[6]=1.0f;	// x2
			vertices[7]=0.0f;	// y2
			vertices[8]=0.0f;	// z2
			
			vertices[9]=1.0f;	// x3
			vertices[10]=1.0f;	// y3
			vertices[11]=0.0f;	// z3
			

			indices = new ushort[indexSize];
			indices[0] = 0;
			indices[1] = 1;
			indices[2] = 2;
			indices[3] = 3;
			
			//												vertex pos,               texture,       color
			vertexBuffer = new VertexBuffer(4, indexSize, VertexFormat.Float3, VertexFormat.Float2, VertexFormat.Float4);
			

			vertexBuffer.SetVertices(0, vertices);
			vertexBuffer.SetVertices(1, texcoords);
			vertexBuffer.SetVertices(2, colors);
			
			vertexBuffer.SetIndices(indices);
			graphics.SetVertexBuffer(0, vertexBuffer);
			
			unitScreenMatrix = new Matrix4(
				 Width*2.0f/rectScreen.Width,	0.0f,	    0.0f, 0.0f,
				 0.0f,   Height*(-2.0f)/rectScreen.Height,	0.0f, 0.0f,
				 0.0f,   0.0f, 1.0f, 0.0f,
				 -1.0f,  1.0f, 0.0f, 1.0f
			);
		}
		
		public static void render(Texture2D texture)
		{
			graphics.SetShaderProgram(shaderProgram);
			graphics.SetVertexBuffer(0, vertexBuffer);
			graphics.SetTexture(0, texture);
			shaderProgram.SetUniformValue(0, ref unitScreenMatrix);
			
			graphics.DrawArrays(DrawMode.TriangleStrip, 0, indexSize);
		}
		
	}
}

