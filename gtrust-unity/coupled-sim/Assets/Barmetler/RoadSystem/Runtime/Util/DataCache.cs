using System;


namespace Barmetler
{
    public class DataCache<T> : InValidatable
    {
        private T data;
        private bool valid = false;


        public void SetData(T data)
        {
            this.data = data;
            valid = true;
        }


        public T GetData()
        {
            if (!IsValid())
            {
                throw new Exception("Cache is invalid");
            }

            return data;
        }


        public override void OnInvalidate()
        {
            valid = false;
        }


        public bool IsValid()
        {
            return valid;
        }
    }
}