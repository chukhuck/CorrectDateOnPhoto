using System.Text.RegularExpressions;

namespace CorrectDateOnPhoto
{
    /// <summary>
    /// A base class is a stratagy for correction date
    /// </summary>
    public abstract class BaseImageDateCorrector : IDisposable
    {
        public const string DATE_PATTERN = @"\d{4}-\d{2}-\d{2} \d{2}-\d{2}-\d{2}";
        private const string DATE_FORMAT = "yyyy-MM-dd HH-mm-ss";
        private const string NEW_FILENAME_POSTFIX = "_edit";
        private const string DIRECTORY_NAME_FOR_NEW_FILENAME_POSTFIX = "EDIT";

        /// <summary>
        /// Old file name
        /// </summary>
        public string OldFilename { get; set; }

        /// <summary>
        /// Name of a new directory
        /// </summary>
        public string NewDirectoryName =>
                Path.Combine( 
                    Path.GetDirectoryName(OldFilename) ?? throw new ArgumentNullException(OldFilename), 
                    DIRECTORY_NAME_FOR_NEW_FILENAME_POSTFIX);

        /// <summary>
        /// New filename
        /// </summary>
        public string NewFilename => 
            Path.GetFileNameWithoutExtension(OldFilename) + 
            NEW_FILENAME_POSTFIX + 
            Path.GetExtension(OldFilename);

        /// <summary>
        /// New full filename
        /// </summary>
        public string NewFullFileName => Path.Combine(NewDirectoryName, NewFilename);

        internal BaseImageDateCorrector()
        {
            throw new ArgumentNullException("filename");
        }

        public BaseImageDateCorrector(string filename)
        {
            if(filename is null)
                throw new ArgumentNullException("filename");

            OldFilename = filename;
        }


        /// <summary>
        /// Main entry point to a correction date. Change EXIF dates and file dates
        /// </summary>
        /// <returns>True if success, false otherwise </returns>
        public bool CorrectDateFor()
        {
            try
            {
                DateTime? trueDate = TryGetTrueDate();

                if (trueDate.HasValue)
                {
                    SetEXIFDate(trueDate);
                    SetFileDate(trueDate);
                }
            }
            catch
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Set Origin, Digitized, Taken dates to date value. Then save it to a new file.
        /// </summary>
        /// <param name="date">New date</param>
        public abstract void SetEXIFDate(DateTime? date);

        /// <summary>
        /// Set CreationTime and LastWriteTime of file to a new date
        /// </summary>
        /// <param name="date">New date</param>
        public virtual void SetFileDate(DateTime? date)
        {
            if (date is null)
            {
                return;
            }

            if (!File.Exists(NewFullFileName))
                return;

            File.SetCreationTime(NewFullFileName, date.Value);
            File.SetLastWriteTime(NewFullFileName, date.Value);
        }

        /// <summary>
        /// Get real date from filename
        /// </summary>
        /// <returns>Real date from filename. If filename does not match to DATE_PATTERN then NULL returns</returns>
        public virtual DateTime? GetDateFromFileName()
        {
            Regex regex = new Regex(DATE_PATTERN);

            Match match = regex.Match(Path.GetFileName(OldFilename));

            if (match.Success)
            {
                string dateAsString = match.Value;
                return DateTime.ParseExact(dateAsString, DATE_FORMAT, null);
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Try to get real date of shot . Method try to get date from OriginDate, FromFilenameDate, DigitizedDate, TakenDate sequantly.
        /// </summary>
        /// <returns>Date is found firstly. Null if no one is found</returns>
        public DateTime? TryGetTrueDate()
        {
            DateTime? originDate =  GetOriginDate();
            DateTime? takenDate = GetTakenDate();
            DateTime? digitizedDate = GetDigitizedDate();
            DateTime? createDateFromFilename = GetDateFromFileName();

            if (originDate.HasValue)
                return originDate;

            if (createDateFromFilename.HasValue)
                return createDateFromFilename;

            if (digitizedDate.HasValue)
                return digitizedDate;

            if (takenDate.HasValue)
                return takenDate;

            return null;

        }

        /// <summary>
        /// Get Digitized Date
        /// </summary>
        /// <returns>Digitized Date if it is found. Otherwise NULL</returns>
        public abstract DateTime? GetDigitizedDate();

        /// <summary>
        /// Get Taken Date
        /// </summary>
        /// <returns>Taken Date if it is found. Otherwise NULL</returns>
        public abstract DateTime? GetTakenDate();

        /// <summary>
        /// Get Origin Date
        /// </summary>
        /// <returns>Origin Date if it is found. Otherwise NULL</returns>
        public abstract DateTime? GetOriginDate();

        public abstract void Dispose();
    }
}
