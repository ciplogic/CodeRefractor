using System;
using System.ComponentModel;
using System.Linq.Expressions;

namespace VisualCompiler
{
    public class NotificationViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        // Create the OnPropertyChanged method to raise the event
        void Changed(string name)
        {
            var handler = PropertyChanged;
            if (handler != null)
                handler(this, new PropertyChangedEventArgs(name));
        }


        public void Changed<T>(Expression<Func<T>> propertyName)
        {
            var body = propertyName.Body as MemberExpression;
            if (body == null)
                throw new ArgumentException("'propertyExpression' should be a member expression");

            Changed(body.Member.Name);
        }
    }
}