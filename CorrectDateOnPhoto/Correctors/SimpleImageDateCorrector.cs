using CorrectDateOnPhoto.Utils;
using System.Drawing;

namespace CorrectDateOnPhoto.Correctors
{
    /// <summary>
    /// This class realize a date correction through System.Drawing.Image
    /// </summary>
    public class SimpleImageDateCorrector : BaseImageDateCorrector
    {
        private Image image;

#pragma warning disable CS8618 
        internal SimpleImageDateCorrector(){}
#pragma warning restore cs8618

        internal SimpleImageDateCorrector(string filename) : base(filename)
        {
            image = Image.FromStream(File.OpenRead(filename), false, false);
        }

        public override void SetEXIFDate(DateTime? date)
        {
            try
            {
                image.SetAllEXIFDateTo(date);
                image.Save(NewFullFileName);
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                image.Dispose();
            }
        }

        public override DateTime? GetDigitizedDate()
        {
            return image.GetDigitizedDate();
        }

        public override DateTime? GetTakenDate()
        {
            return image.GetTakenDate();
        }

        public override DateTime? GetOriginDate()
        {
            return image.GetOriginDate();
        }

        public override void Dispose()
        {
            image.Dispose();
        }
    }
}
