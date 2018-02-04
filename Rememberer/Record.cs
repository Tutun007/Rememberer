using System;

namespace Rememberer
{
    enum Status { New, Edit, DB, Del}

    class Record
    {
        public Record(int newId, string newText, Status newStatus)
        {
            id = newId;
            text = newText;
            status = newStatus;
        }

        public Record(string newText, Status newStatus)
        {
            id = -1;
            text = newText;
            status = newStatus;
        }

        private int id;
        public int Id{
            get { return id; }
            set { id = value; }
        }

        private string text;
        public string Text
        {
            get { return text; }
            set { text = value; }
        }

        private Status status;
        public Status Status
        {
            get { return status; }
            set { status = value; }
        }

        public override string ToString() {
            return String.Format("{0, 3}", id) + 
                " | " + String.Format("{0, -65}", text.Substring(0, text.Length>65?65: text.Length)) +
                " | "+status;
        }
    }
}
