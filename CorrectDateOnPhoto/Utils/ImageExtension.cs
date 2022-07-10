using System.Drawing;
using System.Drawing.Imaging;
using System.Text;

namespace CorrectDateOnPhoto.Utils
{
    /// <summary>
    /// Extension class for System.Drawing.Image
    /// </summary>
    public static class ImageExtension
    {
        private const string TAKEN_DATE_PROPERTY_ID = "0132";
        private const string ORIGIN_DATE_PROPERTY_ID = "9003";
        private const string DIGITIZED_DATE_PROPERTY_ID = "9004";

        private const string DATE_FORMAT = "yyyy:MM:dd HH:mm:ss";

        /// <summary>
        /// Set TakenDate, OriginDate and DigitizedDate to a new date
        /// </summary>
        /// <param name="image">Image</param>
        /// <param name="date">New date</param>
        /// <returns>True if success</returns>
        public static bool SetAllEXIFDateTo(this Image image, DateTime? date)
        {
            if (image == null || date == null)
                return false;

            Encoding? encoding = Encoding.UTF8;

            image.SetTakenDatePropertyItem(date, encoding, forced: true);
            image.SetOriginDatePropertyItem(date, encoding, forced: true);
            image.SetDigitizedDatePropertyItem(date, encoding);

            return true;
        }

        /// <summary>
        /// Set TakenDate of photo to a new date.
        /// </summary>
        /// <param name="image">Image</param>
        /// <param name="date">New date</param>
        /// <param name="encoding">Encoding</param>
        /// <param name="forced">If true and image does not have TakenDateProperties then try to create it</param>
        /// <exception cref="Exception">Exception No properties if the Image does not have any property</exception>
        public static void SetTakenDatePropertyItem(this Image image, DateTime? date, Encoding? encoding, bool forced)
        {
            if (date is null)
                return;

            if (encoding is null)
            {
                encoding = Encoding.UTF8;
            }

            PropertyItem? takenDatePropertyItem = image.GetTakenDatePropertyItem();
            if (takenDatePropertyItem != null)
            {
                takenDatePropertyItem.Value = encoding.GetBytes(date.Value.ToString(DATE_FORMAT) + '\0');
                image.SetPropertyItem(takenDatePropertyItem);
            }
            else if (forced)
            {
                var tempProperty = image.PropertyItems.FirstOrDefault();

                if (tempProperty is null)
                    throw new Exception("No properties");

                tempProperty.Len = 20;
                tempProperty.Id = 306;
                tempProperty.Type = 2;
                tempProperty.Value = encoding.GetBytes(date.Value.ToString(DATE_FORMAT) + '\0');
                image.SetPropertyItem(tempProperty);
            }
        }

        /// <summary>
        /// Set OriginDate of photo to a new date.
        /// </summary>
        /// <param name="image">Image</param>
        /// <param name="date">New date</param>
        /// <param name="encoding">Encoding</param>
        /// <param name="forced">If true and image does not have OriginDateProperties then try to create it</param>
        /// <exception cref="Exception">Exception No properties if the Image does not have any property</exception>
        public static void SetOriginDatePropertyItem(this Image image, DateTime? date, Encoding? encoding, bool forced)
        {
            if (date is null)
                return;

            if (encoding is null)
            {
                encoding = Encoding.UTF8;
            }

            PropertyItem? originDatePropertyItem = image.GetOriginDatePropertyItem();
            
            if (originDatePropertyItem != null)
            {
                originDatePropertyItem.Value = encoding.GetBytes(date.Value.ToString(DATE_FORMAT) + '\0');
                image.SetPropertyItem(originDatePropertyItem);
            }
            else if(forced)
            {
                var tempProperty = image.PropertyItems.FirstOrDefault();

                if (tempProperty is null)
                    throw new Exception("No properties");

                tempProperty.Len = 20;
                tempProperty.Id = 36867;
                tempProperty.Type = 2;
                tempProperty.Value = encoding.GetBytes(date.Value.ToString(DATE_FORMAT) + '\0');
                image.SetPropertyItem(tempProperty);
            }
        }

        /// <summary>
        /// Set DigitizedDate of photo to a new date.
        /// </summary>
        /// <param name="image">Image</param>
        /// <param name="date">New date</param>
        /// <param name="encoding">Encoding</param>
        public static void SetDigitizedDatePropertyItem(this Image image, DateTime? date, Encoding? encoding)
        {
            if (date is null)
                return;

            if (encoding is null)
            {
                encoding = Encoding.UTF8;
            }

            PropertyItem? digitizedDatePropertyItem = image.GetDigitizedDatePropertyItem();
            if (digitizedDatePropertyItem != null)
            {
                digitizedDatePropertyItem.Value = encoding.GetBytes(date.Value.ToString(DATE_FORMAT) + '\0');
                image.SetPropertyItem(digitizedDatePropertyItem);
            }
        }

        /// <summary>
        /// Get Property by Id
        /// </summary>
        /// <param name="image">Image</param>
        /// <param name="id">Needed id</param>
        /// <returns>PropertyItem. Null if needed id does not exist</returns>
        public static PropertyItem? GetPropertyItemById(this Image image, string id)
        {
            if (image == null)
                return null;

            PropertyItem[] propItems = image.PropertyItems;

            var property = propItems.Where(a => a.Id.ToString("x") == id).FirstOrDefault();


            return property;
        }

        /// <summary>
        /// Get TakenDatePropertyItem
        /// </summary>
        /// <param name="image">Image</param>
        /// <returns>TakenDatePropertyItem. NULL if TakenDatePropertyItem is not found</returns>
        public static PropertyItem? GetTakenDatePropertyItem(this Image image)
        {
            if (image == null)
                return null;

            return image.GetPropertyItemById(TAKEN_DATE_PROPERTY_ID);
        }

        /// <summary>
        /// Get OriginDatePropertyItem
        /// </summary>
        /// <param name="image">Image</param>
        /// <returns>OriginDatePropertyItem. NULL if OriginDatePropertyItem is not found</returns>
        public static PropertyItem? GetOriginDatePropertyItem(this Image image)
        {
            if (image == null)
                return null;

            return image.GetPropertyItemById(ORIGIN_DATE_PROPERTY_ID);
        }

        /// <summary>
        /// Get DigitizedDatePropertyItem
        /// </summary>
        /// <param name="image">Image</param>
        /// <returns>DigitizedDatePropertyItem. NULL if DigitizedDatePropertyItem is not found</returns>
        public static PropertyItem? GetDigitizedDatePropertyItem(this Image image)
        {
            if (image == null)
                return null;

            return image.GetPropertyItemById(DIGITIZED_DATE_PROPERTY_ID);
        }

        /// <summary>
        /// Get Origin date
        /// </summary>
        /// <param name="image">Image</param>
        /// <returns>Returns Origin date. NULL if OriginDatePropertyItem is not found</returns>
        public static DateTime? GetOriginDate(this Image image)
        {
            if (image == null)
                return null;

            return image.GetOriginDatePropertyItem()?.GetPropertyItemAsDate(Encoding.UTF8);
        }

        /// <summary>
        /// Get Taken date
        /// </summary>
        /// <param name="image">Image</param>
        /// <returns>Returns Taken date. NULL if TakenDatePropertyItem is not found</returns>
        public static DateTime? GetTakenDate(this Image image)
        {
            if (image == null)
                return null;

            return image.GetTakenDatePropertyItem()?.GetPropertyItemAsDate(Encoding.UTF8);
        }

        /// <summary>
        /// Get Digitized date
        /// </summary>
        /// <param name="image">Image</param>
        /// <returns>Returns Digitized date. NULL if DigitizedDatePropertyItem is not found</returns>
        public static DateTime? GetDigitizedDate(this Image image)
        {
            if (image == null)
                return null;

            return image.GetDigitizedDatePropertyItem()?.GetPropertyItemAsDate(Encoding.UTF8);
        }

        /// <summary>
        /// Convert PropertyItem to date
        /// </summary>
        /// <param name="propertyItem">Image</param>
        /// <param name="encoding"></param>
        /// <returns>Returns PropertyItem as Date. if converting is faled then NULL</returns>
        public static DateTime? GetPropertyItemAsDate(this PropertyItem propertyItem, Encoding encoding)
        {
            DateTime? date;
            string originalDateString = encoding.GetString(propertyItem.Value ?? Array.Empty<byte>());
            originalDateString = originalDateString.Remove(originalDateString.Length - 1);
            date = DateTime.ParseExact(originalDateString, DATE_FORMAT, null);
            return date;
        }
    }
}
