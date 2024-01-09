namespace Borderless
{
    public partial class black_canvas : Form
    {
        public black_canvas()
        {
            // Make the form borderless
            FormBorderStyle = FormBorderStyle.None;

            // Set the window state to maximize
            WindowState = FormWindowState.Maximized;

            // Set the background color to dark
            BackColor = Color.Black;

            // Bring the form to the front
            BringToFront();
        }
    }
}