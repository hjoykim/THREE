using THREE;


namespace THREE.Silk.Example
{
	/**
 * Tessellates the famous Utah teapot database by Martin Newell into triangles.
 *
 * Parameters: size = 50, segments = 10, bottom = true, lid = true, body = true,
 *   fitLid = false, blinn = true
 *
 * size is a relative scale: I've scaled the teapot to fit vertically between -1 and 1.
 * Think of it as a "radius".
 * segments - number of line segments to subdivide each patch edge;
 *   1 is possible but gives degenerates, so two is the real minimum.
 * bottom - boolean, if true (default) then the bottom patches are added. Some consider
 *   adding the bottom heresy, so set this to "false" to adhere to the One True Way.
 * lid - to remove the lid and look inside, set to true.
 * body - to remove the body and leave the lid, set this and "bottom" to false.
 * fitLid - the lid is a tad small in the original. This stretches it a bit so you can't
 *   see the teapot's insides through the gap.
 * blinn - Jim Blinn scaled the original data vertically by dividing by about 1.3 to look
 *   nicer. If you want to see the original teapot, similar to the real-world model, set
 *   this to false. True by default.
 *   See http://en.wikipedia.org/wiki/File:Original_Utah_Teapot.jpg for the original
 *   real-world teapot (from http://en.wikipedia.org/wiki/Utah_teapot).
 *
 * Note that the bottom (the last four patches) is not flat - blame Frank Crow, not me.
 *
 * The teapot should normally be rendered as a double sided object, since for some
 * patches both sides can be seen, e.g., the gap around the lid and inside the spout.
 *
 * Segments 'n' determines the number of triangles output.
 *   Total triangles = 32*2*n*n - 8*n    [degenerates at the top and bottom cusps are deleted]
 *
 *   size_factor   # triangles
 *       1          56
 *       2         240
 *       3         552
 *       4         992
 *
 *      10        6320
 *      20       25440
 *      30       57360
 *
 * Code converted from my ancient SPD software, http://tog.acm.org/resources/SPD/
 * Created for the Udacity course "Interactive Rendering", http://bit.ly/ericity
 * Lesson: https://www.udacity.com/course/viewer#!/c-cs291/l-68866048/m-106482448
 * YouTube video on teapot history: https://www.youtube.com/watch?v=DxMfblPzFNc
 *
 * See https://en.wikipedia.org/wiki/Utah_teapot for the history of the teapot
 *
 */
	public class TeapotBufferGeometry : BufferGeometry
    {
		// 32 * 4 * 4 Bezier spline patches
		int[]  teapotPatches =  new int[] {
			/*rim*/
			0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15,
			3, 16, 17, 18, 7, 19, 20, 21, 11, 22, 23, 24, 15, 25, 26, 27,
			18, 28, 29, 30, 21, 31, 32, 33, 24, 34, 35, 36, 27, 37, 38, 39,
			30, 40, 41, 0, 33, 42, 43, 4, 36, 44, 45, 8, 39, 46, 47, 12,
			/*body*/
			12, 13, 14, 15, 48, 49, 50, 51, 52, 53, 54, 55, 56, 57, 58, 59,
			15, 25, 26, 27, 51, 60, 61, 62, 55, 63, 64, 65, 59, 66, 67, 68,
			27, 37, 38, 39, 62, 69, 70, 71, 65, 72, 73, 74, 68, 75, 76, 77,
			39, 46, 47, 12, 71, 78, 79, 48, 74, 80, 81, 52, 77, 82, 83, 56,
			56, 57, 58, 59, 84, 85, 86, 87, 88, 89, 90, 91, 92, 93, 94, 95,
			59, 66, 67, 68, 87, 96, 97, 98, 91, 99, 100, 101, 95, 102, 103, 104,
			68, 75, 76, 77, 98, 105, 106, 107, 101, 108, 109, 110, 104, 111, 112, 113,
			77, 82, 83, 56, 107, 114, 115, 84, 110, 116, 117, 88, 113, 118, 119, 92,
			/*handle*/
			120, 121, 122, 123, 124, 125, 126, 127, 128, 129, 130, 131, 132, 133, 134, 135,
			123, 136, 137, 120, 127, 138, 139, 124, 131, 140, 141, 128, 135, 142, 143, 132,
			132, 133, 134, 135, 144, 145, 146, 147, 148, 149, 150, 151, 68, 152, 153, 154,
			135, 142, 143, 132, 147, 155, 156, 144, 151, 157, 158, 148, 154, 159, 160, 68,
			/*spout*/
			161, 162, 163, 164, 165, 166, 167, 168, 169, 170, 171, 172, 173, 174, 175, 176,
			164, 177, 178, 161, 168, 179, 180, 165, 172, 181, 182, 169, 176, 183, 184, 173,
			173, 174, 175, 176, 185, 186, 187, 188, 189, 190, 191, 192, 193, 194, 195, 196,
			176, 183, 184, 173, 188, 197, 198, 185, 192, 199, 200, 189, 196, 201, 202, 193,
			/*lid*/
			203, 203, 203, 203, 204, 205, 206, 207, 208, 208, 208, 208, 209, 210, 211, 212,
			203, 203, 203, 203, 207, 213, 214, 215, 208, 208, 208, 208, 212, 216, 217, 218,
			203, 203, 203, 203, 215, 219, 220, 221, 208, 208, 208, 208, 218, 222, 223, 224,
			203, 203, 203, 203, 221, 225, 226, 204, 208, 208, 208, 208, 224, 227, 228, 209,
			209, 210, 211, 212, 229, 230, 231, 232, 233, 234, 235, 236, 237, 238, 239, 240,
			212, 216, 217, 218, 232, 241, 242, 243, 236, 244, 245, 246, 240, 247, 248, 249,
			218, 222, 223, 224, 243, 250, 251, 252, 246, 253, 254, 255, 249, 256, 257, 258,
			224, 227, 228, 209, 252, 259, 260, 229, 255, 261, 262, 233, 258, 263, 264, 237,
			/*bottom*/
			265, 265, 265, 265, 266, 267, 268, 269, 270, 271, 272, 273, 92, 119, 118, 113,
			265, 265, 265, 265, 269, 274, 275, 276, 273, 277, 278, 279, 113, 112, 111, 104,
			265, 265, 265, 265, 276, 280, 281, 282, 279, 283, 284, 285, 104, 103, 102, 95,
			265, 265, 265, 265, 282, 286, 287, 266, 285, 288, 289, 270, 95, 94, 93, 92
		};
		float[] teapotVertices = new float[] {
		1.4f, 0f, 2.4f,
		1.4f, -0.784f, 2.4f,
		0.784f, -1.4f, 2.4f,
		0f, -1.4f, 2.4f,
		1.3375f, 0f, 2.53125f,
		1.3375f, -0.749f, 2.53125f,
		0.749f, -1.3375f, 2.53125f,
		0f, -1.3375f, 2.53125f,
		1.4375f, 0f, 2.53125f,
		1.4375f, -0.805f, 2.53125f,
		0.805f, -1.4375f, 2.53125f,
		0f, -1.4375f, 2.53125f,
		1.5f, 0f, 2.4f,
		1.5f, -0.84f, 2.4f,
		0.84f, -1.5f, 2.4f,
		0f, -1.5f, 2.4f,
		-0.784f, -1.4f, 2.4f,
		-1.4f, -0.784f, 2.4f,
		-1.4f, 0f, 2.4f,
		-0.749f, -1.3375f, 2.53125f,
		-1.3375f, -0.749f, 2.53125f,
		-1.3375f, 0f, 2.53125f,
		-0.805f, -1.4375f, 2.53125f,
		-1.4375f, -0.805f, 2.53125f,
		-1.4375f, 0f, 2.53125f,
		-0.84f, -1.5f, 2.4f,
		-1.5f, -0.84f, 2.4f,
		-1.5f, 0f, 2.4f,
		-1.4f, 0.784f, 2.4f,
		-0.784f, 1.4f, 2.4f,
		0f, 1.4f, 2.4f,
		-1.3375f, 0.749f, 2.53125f,
		-0.749f, 1.3375f, 2.53125f,
		0f, 1.3375f, 2.53125f,
		-1.4375f, 0.805f, 2.53125f,
		-0.805f, 1.4375f, 2.53125f,
		0f, 1.4375f, 2.53125f,
		-1.5f, 0.84f, 2.4f,
		-0.84f, 1.5f, 2.4f,
		0f, 1.5f, 2.4f,
		0.784f, 1.4f, 2.4f,
		1.4f, 0.784f, 2.4f,
		0.749f, 1.3375f, 2.53125f,
		1.3375f, 0.749f, 2.53125f,
		0.805f, 1.4375f, 2.53125f,
		1.4375f, 0.805f, 2.53125f,
		0.84f, 1.5f, 2.4f,
		1.5f, 0.84f, 2.4f,
		1.75f, 0f, 1.875f,
		1.75f, -0.98f, 1.875f,
		0.98f, -1.75f, 1.875f,
		0f, -1.75f, 1.875f,
		2f, 0f, 1.35f,
		2f, -1.12f, 1.35f,
		1.12f, -2f, 1.35f,
		0f, -2f, 1.35f,
		2f, 0f, 0.9f,
		2f, -1.12f, 0.9f,
		1.12f, -2f, 0.9f,
		0f, -2f, 0.9f,
		-0.98f, -1.75f, 1.875f,
		-1.75f, -0.98f, 1.875f,
		-1.75f, 0f, 1.875f,
		-1.12f, -2f, 1.35f,
		-2f, -1.12f, 1.35f,
		-2f, 0f, 1.35f,
		-1.12f, -2f, 0.9f,
		-2f, -1.12f, 0.9f,
		-2f, 0f, 0.9f,
		-1.75f, 0.98f, 1.875f,
		-0.98f, 1.75f, 1.875f,
		0f, 1.75f, 1.875f,
		-2f, 1.12f, 1.35f,
		-1.12f, 2f, 1.35f,
		0f, 2f, 1.35f,
		-2f, 1.12f, 0.9f,
		-1.12f, 2f, 0.9f,
		0f, 2f, 0.9f,
		0.98f, 1.75f, 1.875f,
		1.75f, 0.98f, 1.875f,
		1.12f, 2f, 1.35f,
		2f, 1.12f, 1.35f,
		1.12f, 2f, 0.9f,
		2f, 1.12f, 0.9f,
		2f, 0f, 0.45f,
		2f, -1.12f, 0.45f,
		1.12f, -2f, 0.45f,
		0f, -2f, 0.45f,
		1.5f, 0f, 0.225f,
		1.5f, -0.84f, 0.225f,
		0.84f, -1.5f, 0.225f,
		0f, -1.5f, 0.225f,
		1.5f, 0f, 0.15f,
		1.5f, -0.84f, 0.15f,
		0.84f, -1.5f, 0.15f,
		0f, -1.5f, 0.15f,
		-1.12f, -2f, 0.45f,
		-2f, -1.12f, 0.45f,
		-2f, 0f, 0.45f,
		-0.84f, -1.5f, 0.225f,
		-1.5f, -0.84f, 0.225f,
		-1.5f, 0f, 0.225f,
		-0.84f, -1.5f, 0.15f,
		-1.5f, -0.84f, 0.15f,
		-1.5f, 0f, 0.15f,
		-2f, 1.12f, 0.45f,
		-1.12f, 2f, 0.45f,
		0f, 2f, 0.45f,
		-1.5f, 0.84f, 0.225f,
		-0.84f, 1.5f, 0.225f,
		0f, 1.5f, 0.225f,
		-1.5f, 0.84f, 0.15f,
		-0.84f, 1.5f, 0.15f,
		0f, 1.5f, 0.15f,
		1.12f, 2f, 0.45f,
		2f, 1.12f, 0.45f,
		0.84f, 1.5f, 0.225f,
		1.5f, 0.84f, 0.225f,
		0.84f, 1.5f, 0.15f,
		1.5f, 0.84f, 0.15f,
		-1.6f, 0f, 2.025f,
		-1.6f, -0.3f, 2.025f,
		-1.5f, -0.3f, 2.25f,
		-1.5f, 0f, 2.25f,
		-2.3f, 0f, 2.025f,
		-2.3f, -0.3f, 2.025f,
		-2.5f, -0.3f, 2.25f,
		-2.5f, 0f, 2.25f,
		-2.7f, 0f, 2.025f,
		-2.7f, -0.3f, 2.025f,
		-3f, -0.3f, 2.25f,
		-3f, 0f, 2.25f,
		-2.7f, 0f, 1.8f,
		-2.7f, -0.3f, 1.8f,
		-3f, -0.3f, 1.8f,
		-3f, 0f, 1.8f,
		-1.5f, 0.3f, 2.25f,
		-1.6f, 0.3f, 2.025f,
		-2.5f, 0.3f, 2.25f,
		-2.3f, 0.3f, 2.025f,
		-3f, 0.3f, 2.25f,
		-2.7f, 0.3f, 2.025f,
		-3f, 0.3f, 1.8f,
		-2.7f, 0.3f, 1.8f,
		-2.7f, 0f, 1.575f,
		-2.7f, -0.3f, 1.575f,
		-3f, -0.3f, 1.35f,
		-3f, 0f, 1.35f,
		-2.5f, 0f, 1.125f,
		-2.5f, -0.3f, 1.125f,
		-2.65f, -0.3f, 0.9375f,
		-2.65f, 0f, 0.9375f,
		-2f, -0.3f, 0.9f,
		-1.9f, -0.3f, 0.6f,
		-1.9f, 0f, 0.6f,
		-3f, 0.3f, 1.35f,
		-2.7f, 0.3f, 1.575f,
		-2.65f, 0.3f, 0.9375f,
		-2.5f, 0.3f, 1.125f,
		-1.9f, 0.3f, 0.6f,
		-2f, 0.3f, 0.9f,
		1.7f, 0f, 1.425f,
		1.7f, -0.66f, 1.425f,
		1.7f, -0.66f, 0.6f,
		1.7f, 0f, 0.6f,
		2.6f, 0f, 1.425f,
		2.6f, -0.66f, 1.425f,
		3.1f, -0.66f, 0.825f,
		3.1f, 0f, 0.825f,
		2.3f, 0f, 2.1f,
		2.3f, -0.25f, 2.1f,
		2.4f, -0.25f, 2.025f,
		2.4f, 0f, 2.025f,
		2.7f, 0f, 2.4f,
		2.7f, -0.25f, 2.4f,
		3.3f, -0.25f, 2.4f,
		3.3f, 0f, 2.4f,
		1.7f, 0.66f, 0.6f,
		1.7f, 0.66f, 1.425f,
		3.1f, 0.66f, 0.825f,
		2.6f, 0.66f, 1.425f,
		2.4f, 0.25f, 2.025f,
		2.3f, 0.25f, 2.1f,
		3.3f, 0.25f, 2.4f,
		2.7f, 0.25f, 2.4f,
		2.8f, 0f, 2.475f,
		2.8f, -0.25f, 2.475f,
		3.525f, -0.25f, 2.49375f,
		3.525f, 0f, 2.49375f,
		2.9f, 0f, 2.475f,
		2.9f, -0.15f, 2.475f,
		3.45f, -0.15f, 2.5125f,
		3.45f, 0f, 2.5125f,
		2.8f, 0f, 2.4f,
		2.8f, -0.15f, 2.4f,
		3.2f, -0.15f, 2.4f,
		3.2f, 0f, 2.4f,
		3.525f, 0.25f, 2.49375f,
		2.8f, 0.25f, 2.475f,
		3.45f, 0.15f, 2.5125f,
		2.9f, 0.15f, 2.475f,
		3.2f, 0.15f, 2.4f,
		2.8f, 0.15f, 2.4f,
		0f, 0f, 3.15f,
		0.8f, 0f, 3.15f,
		0.8f, -0.45f, 3.15f,
		0.45f, -0.8f, 3.15f,
		0f, -0.8f, 3.15f,
		0f, 0f, 2.85f,
		0.2f, 0f, 2.7f,
		0.2f, -0.112f, 2.7f,
		0.112f, -0.2f, 2.7f,
		0f, -0.2f, 2.7f,
		-0.45f, -0.8f, 3.15f,
		-0.8f, -0.45f, 3.15f,
		-0.8f, 0f, 3.15f,
		-0.112f, -0.2f, 2.7f,
		-0.2f, -0.112f, 2.7f,
		-0.2f, 0f, 2.7f,
		-0.8f, 0.45f, 3.15f,
		-0.45f, 0.8f, 3.15f,
		0f, 0.8f, 3.15f,
		-0.2f, 0.112f, 2.7f,
		-0.112f, 0.2f, 2.7f,
		0f, 0.2f, 2.7f,
		0.45f, 0.8f, 3.15f,
		0.8f, 0.45f, 3.15f,
		0.112f, 0.2f, 2.7f,
		0.2f, 0.112f, 2.7f,
		0.4f, 0f, 2.55f,
		0.4f, -0.224f, 2.55f,
		0.224f, -0.4f, 2.55f,
		0f, -0.4f, 2.55f,
		1.3f, 0f, 2.55f,
		1.3f, -0.728f, 2.55f,
		0.728f, -1.3f, 2.55f,
		0f, -1.3f, 2.55f,
		1.3f, 0f, 2.4f,
		1.3f, -0.728f, 2.4f,
		0.728f, -1.3f, 2.4f,
		0f, -1.3f, 2.4f,
		-0.224f, -0.4f, 2.55f,
		-0.4f, -0.224f, 2.55f,
		-0.4f, 0f, 2.55f,
		-0.728f, -1.3f, 2.55f,
		-1.3f, -0.728f, 2.55f,
		-1.3f, 0f, 2.55f,
		-0.728f, -1.3f, 2.4f,
		-1.3f, -0.728f, 2.4f,
		-1.3f, 0f, 2.4f,
		-0.4f, 0.224f, 2.55f,
		-0.224f, 0.4f, 2.55f,
		0f, 0.4f, 2.55f,
		-1.3f, 0.728f, 2.55f,
		-0.728f, 1.3f, 2.55f,
		0f, 1.3f, 2.55f,
		-1.3f, 0.728f, 2.4f,
		-0.728f, 1.3f, 2.4f,
		0f, 1.3f, 2.4f,
		0.224f, 0.4f, 2.55f,
		0.4f, 0.224f, 2.55f,
		0.728f, 1.3f, 2.55f,
		1.3f, 0.728f, 2.55f,
		0.728f, 1.3f, 2.4f,
		1.3f, 0.728f, 2.4f,
		0f, 0f, 0f,
		1.425f, 0f, 0f,
		1.425f, 0.798f, 0f,
		0.798f, 1.425f, 0f,
		0f, 1.425f, 0f,
		1.5f, 0f, 0.075f,
		1.5f, 0.84f, 0.075f,
		0.84f, 1.5f, 0.075f,
		0f, 1.5f, 0.075f,
		-0.798f, 1.425f, 0f,
		-1.425f, 0.798f, 0f,
		-1.425f, 0f, 0f,
		-0.84f, 1.5f, 0.075f,
		-1.5f, 0.84f, 0.075f,
		-1.5f, 0f, 0.075f,
		-1.425f, -0.798f, 0f,
		-0.798f, -1.425f, 0f,
		0f, -1.425f, 0f,
		-1.5f, -0.84f, 0.075f,
		-0.84f, -1.5f, 0.075f,
		0f, -1.5f, 0.075f,
		0.798f, -1.425f, 0f,
		1.425f, -0.798f, 0f,
		0.84f, -1.5f, 0.075f,
		1.5f, -0.84f, 0.075f
		};

		public TeapotBufferGeometry(int? size=null,int? segments=null,bool? bottom=null,bool? lid=null,bool? body=null, bool? fitLid=null, bool? blinn = null) : base()
        {
			size = size != null ? size : 50;

			// number of segments per patch
			segments = segments != null ? (int)System.Math.Max(2, System.Math.Floor((decimal)segments.Value)) : 10; 

			// which parts should be visible
			bottom = bottom == null ? true : bottom;
			lid = lid == null ? true : lid;
			body = body == null ? true : body;

			// Should the lid be snug? It's not traditional, but we make it snug by default
			fitLid = fitLid == null ? true : fitLid;

			// Jim Blinn scaled the teapot down in size by about 1.3 for
			// some rendering tests. He liked the new proportions that he kept
			// the data in this form. The model was distributed with these new
			// proportions and became the norm. Trivia: comparing images of the
			// real teapot and the computer model, the ratio for the bowl of the
			// real teapot is more like 1.25, but since 1.3 is the traditional
			// value given, we use it here.
			var blinnScale = 1.3f;
			blinn = blinn == null ? true : blinn;

			// scale the size to be the real scaling factor
			var maxHeight = 3.15f * (blinn.Value ? 1 : blinnScale);

			var maxHeight2 = maxHeight / 2;
			var trueSize = size.Value/ maxHeight2;

			// Number of elements depends on what is needed. Subtract degenerate
			// triangles at tip of bottom and lid out in advance.
			var numTriangles = bottom.Value ? (8 * segments.Value - 4) * segments.Value : 0;
			numTriangles += lid.Value ? (16 * segments.Value - 4) * segments.Value : 0;
			numTriangles += body.Value ? 40 * segments.Value * segments.Value : 0;

			var indices = new int[numTriangles * 3];

			var numVertices = bottom.Value ? 4 : 0;
			numVertices += lid.Value ? 8 : 0;
			numVertices += body.Value ? 20 : 0;
			numVertices *= (segments.Value + 1) * (segments.Value + 1);

			var vertices = new float[numVertices * 3];
			var normals = new float[numVertices * 3];
			var uvs = new float[numVertices * 2];

			// Bezier form
			var ms = new Matrix4();
			ms.Set(
				-1.0f, 3.0f, -3.0f, 1.0f,
				3.0f, -6.0f, 3.0f, 0.0f,
				-3.0f, 3.0f, 0.0f, 0.0f,
				1.0f, 0.0f, 0.0f, 0.0f);

			

			// M * G * M matrix, sort of see
			// http://www.cs.helsinki.fi/group/goa/mallinnus/curves/surfaces.html
			var mgm = new Matrix4[3];

			var vert = new float[3];
			var sdir = new float[3];
			var tdir = new float[3];

			var norm = new Vector3();

			Vector4 tcoord;

			int sstep, tstep;
			int vertPerRow;

			float s, t;
			int p;
			float sval, tval;
			float dsval = 0;
			float dtval = 0;

			var normOut = new Vector3();
			int v1, v2, v3, v4;

			var gmx = new Matrix4();
			var tmtx = new Matrix4();

			var vsp = new Vector4();
			var vtp = new Vector4();
			var vdsp = new Vector4();
			var vdtp = new Vector4();

			var vsdir = new Vector3();
			var vtdir = new Vector3();

			var mst = (Matrix4)ms.Clone();
			mst.Transpose();

			
			int i, r, c;

			

			for (i = 0; i < 3; i++)
			{

				mgm[i] = new Matrix4();

			}
			var minPatches = body.Value ? 0 : 20;
			var maxPatches = bottom.Value ? 32 : 28;

			vertPerRow = segments.Value + 1;

			var surfCount = 0;

			var vertCount = 0;
			var normCount = 0;
			var uvCount = 0;

			var indexCount = 0;

			var g = new float[16];
			var sp =new float[4];
			var tp =new float[4];
			var dsp = new float[4];
			var dtp = new float[4];
			for (var surf = minPatches; surf < maxPatches; surf++)
			{
				
				// lid is in the middle of the data, patches 20-27,
				// so ignore it for this part of the loop if the lid is not desired
				if (lid.Value || (surf < 20 || surf >= 28))
				{

					// get M * G * M matrix for x,y,z
					for (i = 0; i < 3; i++)
					{

						// get control patches
						for (r = 0; r < 4; r++)
						{

							for (c = 0; c < 4; c++)
							{

								// transposed
								g[c * 4 + r] = teapotVertices[teapotPatches[surf * 16 + r * 4 + c] * 3 + i];

								// is the lid to be made larger, and is this a point on the lid
								// that is X or Y?
								if (fitLid.Value && (surf >= 20 && surf < 28) && (i != 2))
								{

									// increase XY size by 7.7%, found empirically. I don't
									// increase Z so that the teapot will continue to fit in the
									// space -1 to 1 for Y (Y is up for the final model).
									g[c * 4 + r] *= 1.077f;

								}

								// Blinn "fixed" the teapot by dividing Z by blinnScale, and that's the
								// data we now use. The original teapot is taller. Fix it:
								if (!(blinn.Value) && (i == 2))
								{

									g[c * 4 + r] *= blinnScale;

								}

							}

						}

						gmx.Set(g[0], g[1], g[2], g[3], g[4], g[5], g[6], g[7], g[8], g[9], g[10], g[11], g[12], g[13], g[14], g[15]);

						tmtx.MultiplyMatrices(gmx, ms);
						mgm[i].MultiplyMatrices(mst, tmtx);

					}

					// step along, get points, and output
					for (sstep = 0; sstep <= segments; sstep++)
					{

						s = 1.0f*sstep / segments.Value;

						for (tstep = 0; tstep <= segments; tstep++)
						{

							t = 1.0f*tstep / segments.Value;

							// point from basis
							// get power vectors and their derivatives
							sval = 1.0f;
							tval = 1.0f;
							
							for (p=3; p >= 0; p--)
							{

								sp[p] = sval;
								tp[p] = tval;
								sval *= s;
								tval *= t;

								if (p == 3)
								{

									dsp[p] = dtp[p] = 0.0f;
									dsval = 1.0f;
									dtval = 1.0f;

								}
								else
								{

									dsp[p] = dsval * (3 - p);
									dtp[p] = dtval * (3 - p);
									dsval *= s;
									dtval *= t;

								}

							}

							vsp.FromArray(sp);
							vtp.FromArray(tp);
							vdsp.FromArray(dsp);
							vdtp.FromArray(dtp);

							

							// do for x,y,z
							for (i = 0; i < 3; i++)
							{

								// multiply power vectors times matrix to get value
								tcoord = (Vector4)vsp.Clone();
								tcoord.ApplyMatrix4(mgm[i]);
								vert[i] = tcoord.Dot(vtp);

								// get s and t tangent vectors
								tcoord = (Vector4)vdsp.Clone();
								tcoord.ApplyMatrix4(mgm[i]);
								sdir[i] = tcoord.Dot(vtp);

								tcoord = (Vector4)vsp.Clone();
								tcoord.ApplyMatrix4(mgm[i]);
								tdir[i] = tcoord.Dot(vdtp);

							}

							// find normal
							vsdir.FromArray(sdir);
							vtdir.FromArray(tdir);
							norm.CrossVectors(vtdir, vsdir);
							norm.Normalize();

							// if X and Z length is 0, at the cusp, so point the normal up or down, depending on patch number
							if (vert[0] == 0 && vert[1] == 0)
							{

								// if above the middle of the teapot, normal points up, else down
								normOut.Set(0, vert[2] > maxHeight2 ? 1 : -1, 0);

							}
							else
							{

								// standard output: rotate on X axis
								normOut.Set(norm.X, norm.Z, -norm.Y);

							}
							
							// store it all
							vertices[vertCount++] = trueSize * vert[0];
							vertices[vertCount++] = trueSize * (vert[2] - maxHeight2);
							vertices[vertCount++] = -trueSize * vert[1];

							normals[normCount++] = normOut.X;
							normals[normCount++] = normOut.Y;
							normals[normCount++] = normOut.Z;

							uvs[uvCount++] = 1 - t;
							uvs[uvCount++] = 1 - s;

						}

					}

					// save the faces
					for (sstep = 0; sstep < segments; sstep++)
					{

						for (tstep = 0; tstep < segments; tstep++)
						{

							v1 = surfCount * vertPerRow * vertPerRow + sstep * vertPerRow + tstep;
							v2 = v1 + 1;
							v3 = v2 + vertPerRow;
							v4 = v1 + vertPerRow;

							// Normals and UVs cannot be shared. Without clone(), you can see the consequences
							// of sharing if you call geometry.applyMatrix4( matrix ).
							if (NotDegenerate(vertices,v1, v2, v3))
							{
								if (indexCount < indices.Length)
								{
									indices[indexCount++] = v1;
									indices[indexCount++] = v2;
									indices[indexCount++] = v3;
								}

							}

							if (NotDegenerate(vertices,v1, v3, v4))
							{
								if (indexCount < indices.Length)
								{
									indices[indexCount++] = v1;
									indices[indexCount++] = v3;
									indices[indexCount++] = v4;
								}

							}

						}

					}

					// increment only if a surface was used
					surfCount++;

				}

			}

            SetIndex(new BufferAttribute<int>(indices, 1));
            SetAttribute("position", new BufferAttribute<float>(vertices, 3));
			SetAttribute("normal", new BufferAttribute<float>(normals, 3));
			SetAttribute("uv", new BufferAttribute<float>(uvs, 2));

			ComputeBoundingSphere();
		}
		private bool NotDegenerate(float[] vertices,int vtx1,int vtx2,int vtx3)
        {
			// if any vertex matches, return false			
			return !(((vertices[vtx1 * 3] == vertices[vtx2 * 3]) && (vertices[vtx1 * 3 + 1] == vertices[vtx2 * 3 + 1]) && (vertices[vtx1 * 3 + 2] == vertices[vtx2 * 3 + 2])) ||
					   ((vertices[vtx1 * 3] == vertices[vtx3 * 3]) && (vertices[vtx1 * 3 + 1] == vertices[vtx3 * 3 + 1]) && (vertices[vtx1 * 3 + 2] == vertices[vtx3 * 3 + 2])) ||
					   ((vertices[vtx2 * 3] == vertices[vtx3 * 3]) && (vertices[vtx2 * 3 + 1] == vertices[vtx3 * 3 + 1]) && (vertices[vtx2 * 3 + 2] == vertices[vtx3 * 3 + 2])));
		}
    }
}
