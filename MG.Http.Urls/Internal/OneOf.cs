using System.Diagnostics.CodeAnalysis;

namespace MG.Http.Urls.Internal
{
    internal readonly struct OneOf<T0, T1> : IEquatable<OneOf<T0, T1>>
    {
        static readonly Type _stringType = typeof(string);
        readonly bool _isItem0;
        readonly bool _isItem1;
        readonly bool _isT0String;
        readonly bool _isT1String;

        readonly T0? _item0;
        readonly T1? _item1;

        [MemberNotNullWhen(true, nameof(_item0))]
        internal bool IsT0 => _isItem0;
        [MemberNotNullWhen(true, nameof(_item1))]
        internal bool IsT1 => _isItem1;

        internal T0 AsT0 => _item0 ?? throw new InvalidOperationException("Value is not T0.");
        internal T1 AsT1 => _item1 ?? throw new InvalidOperationException("Value is not T1.");

        private OneOf(T0? item0, T1? item1, bool isItem0, bool isItem1, bool isT0String, bool isT1String)
        {
            _item0 = item0;
            _item1 = item1;
            _isItem0 = isItem0;
            _isItem1 = isItem1;
            _isT0String = isT0String;
            _isT1String = isT1String;
        }

        public bool Equals(OneOf<T0, T1> other)
        {
            if (this.IsT0 && other.IsT0)
            {
                return EqualityComparer<T0>.Default.Equals(_item0, other._item0);
            }
            else if (this.IsT1 && other.IsT1)
            {
                return EqualityComparer<T1>.Default.Equals(_item1, other._item1);
            }
            else
            {
                return false;
            }
        }
        public override bool Equals([NotNullWhen(true)] object? obj)
        {
            if (obj is OneOf<T0, T1> other)
            {
                return this.Equals(other);
            }
            else
            {
                return false;
            }
        }
        public static OneOf<T0, T1> FromT0(T0 item0)
        {
            return new(item0, default, true, false, _stringType.Equals(typeof(T0)), false);
        }
        public static OneOf<T0, T1> FromT1(T1 item1)
        {
            return new(default, item1, false, true, false, _stringType.Equals(typeof(T1)));
        }
        public override int GetHashCode()
        {
            if (this.IsT0)
            {
                return GetObjectHashCode(_item0, _isT0String);
            }
            else if (this.IsT1)
            {
                return GetObjectHashCode(_item1, _isT1String);
            }
            else
            {
                return this.GetHashCode();
            }
        }

        private static int GetObjectHashCode(object o, bool isStringType)
        {
            return isStringType
                ? StringComparer.InvariantCultureIgnoreCase.GetHashCode((string)o)
                : o.GetHashCode();
        }

        internal bool TryPickT0([NotNullWhen(true)] out T0? value, [NotNullWhen(false)] out T1? remainder)
        {
            if (this.IsT0)
            {
                value = _item0;
                remainder = default;
                return true;
            }
            else
            {
                value = default;
                remainder = _item1 ?? throw new InvalidOperationException("Value is neither T0 or T1.");
                return false;
            }
        }
        internal bool TryPickT1([NotNullWhen(true)] out T1? value, [NotNullWhen(false)] out T0? remainder)
        {
            if (this.IsT1)
            {
                value = _item1;
                remainder = default;
                return true;
            }
            else
            {
                value = default;
                remainder = _item0 ?? throw new InvalidOperationException("Value is neither T0 or T1.");
                return false;
            }
        }

        public static bool operator ==(OneOf<T0, T1> left, OneOf<T0, T1> right)
        {
            return left.Equals(right);
        }
        public static bool operator !=(OneOf<T0, T1> left, OneOf<T0, T1> right)
        {
            return !(left == right);
        }
    }
}

