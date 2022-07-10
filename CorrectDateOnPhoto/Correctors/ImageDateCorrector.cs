using ExifLibrary;

namespace CorrectDateOnPhoto.Correctors
{
    /// <summary>
    /// This class realize a date correction using ExifLibrary
    /// </summary>
    public class ImageDateCorrector : BaseImageDateCorrector
    {
        private ImageFile image;

#pragma warning disable CS8618
        internal ImageDateCorrector() { }
#pragma warning restore CS8618

        internal ImageDateCorrector(string filename) : base(filename)
        {
            image = ImageFile.FromFile(filename);
        }

        public override void Dispose()
        {
        }

        public override DateTime? GetDigitizedDate()
        {
            return image.Properties.Get<ExifDateTime>(ExifTag.DateTimeDigitized)?.Value;
        }

        public override DateTime? GetOriginDate()
        {
            return image.Properties.Get<ExifDateTime>(ExifTag.DateTimeOriginal)?.Value;
        }

        public override DateTime? GetTakenDate()
        {
            return image.Properties.Get<ExifDateTime>(ExifTag.DateTime)?.Value;
        }

        public override void SetEXIFDate(DateTime? date)
        {
            if (date is null)
                return;

            SetDateTimeToExif(date.Value, ExifTag.DateTime, forced: true);
            SetDateTimeToExif(date.Value, ExifTag.DateTimeDigitized, forced: false);
            SetDateTimeToExif(date.Value, ExifTag.DateTimeOriginal, forced: true);

            image.Save(NewFullFileName);
        }

        private void SetDateTimeToExif(DateTime date, ExifTag tag, bool forced)
        {
            if (GetTakenDate() is not null)
            {
                image.Properties.Set(ExifTag.DateTime, new ExifDateTime(tag, date));
            }
            else if (forced)
            {
                image.Properties.Add(ExifTag.DateTime, new ExifDateTime(tag, date));
            }
        }
    }
}
