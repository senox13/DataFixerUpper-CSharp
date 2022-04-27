using System;
using System.Collections;
using System.Collections.Generic;

namespace DataFixerUpper.Util{
    /// <summary>
    /// A container object which may or may not contain a non-<c>null</c>
    /// value. If a value is present, <see cref="IsPresent"/> returns
    /// <c>true</c>. If no value is present, the object is considered empty
    /// and <see cref="IsPresent"/> returns <c>false</c>.
    /// 
    /// Additional methods that depend on the presence or absence of a
    /// contained value are provided, such as <see cref="OrElse"/> (returns a
    /// default value if no value is present) and <see cref="IsPresent"/>
    /// (performs an action if a value is present).
    /// </summary>
    /// <typeparam name="T">The type of the value contained in this Optional</typeparam>
    public struct Optional<T> : IEnumerable<T>{
        /*
         * Fields
         */
        private readonly static Optional<T> EMPTY = new Optional<T>(default, true);
        private readonly bool isEmpty;
        private readonly T value;


        /*
         * Constructor
         */
        private Optional(T valueIn, bool isEmptyIn){
            value = valueIn;
            isEmpty = isEmptyIn;
        }

        private Optional(T valueIn) : this(valueIn, false){
            if(valueIn == null){
                throw new ArgumentNullException(nameof(valueIn), "Cannot instantiate Optional<T> with null value, use Optional<T>.Empty() instead");
            }
        }


        /*
         * Static methods
         */
        /// <summary>
        /// Returns an empty <see cref="Optional{T}"/> instance.
        /// </summary>
        /// <returns>Returns an empty Optional</returns>
        public static Optional<T> Empty(){
            return EMPTY;
        }

        /// <summary>
        /// Returns an <see cref="Optional{T}"/> containing the given value.
        /// If the value given is <c>null</c>, an
        /// <see cref="ArgumentNullException"/> will be thrown.
        /// </summary>
        /// <param name="valueIn">The value to wrap in an optional</param>
        /// <returns>An optional containing the given value</returns>
        public static Optional<T> Of(T valueIn){
            return new Optional<T>(valueIn);
        }

        /// <summary>
        /// Returns an <see cref="Optional{T}"/> containing the given value.
        /// If the value given is <c>null</c>, an empty optional will be
        /// returned.
        /// </summary>
        /// <param name="valueIn">The value to wrap in an optional</param>
        /// <returns>An optional containing the given value, or an empty
        /// optional if the given value is <c>null</c></returns>
        public static Optional<T> OfNullable(T valueIn){
            if(valueIn == null){
                return EMPTY;
            }
            return new Optional<T>(valueIn);
        }


        /*
         * Instance methods
         */
        /// <summary>
        /// Gets the value contained in this <see cref="Optional{T}"/>, if
        /// present. If this optional is empty, throws an
        /// <see cref="InvalidOperationException"/>.
        /// </summary>
        /// <remarks>The preferred alternative to this method is
        /// <see cref="OrElseThrow"/></remarks>
        /// <returns>The non-<c>null</c> value in this optional</returns>
        public T Get(){
            if(IsEmpty()){
                throw new InvalidOperationException("No value present");
            }
            return value;
        }

        /// <summary>
        /// Checks if a value is present in this <see cref="Optional{T}"/>.
        /// </summary>
        /// <returns><c>true</c> if there is a value present in this
        /// optional, otherwise <c>false</c></returns>
        public bool IsPresent(){
            return !isEmpty;
        }

        /// <summary>
        /// Checks if this <see cref="Optional{T}"/> is empty.
        /// </summary>
        /// <returns><c>false</c> if there is a value present in this
        /// optional, otherwise <c>true</c></returns>
        public bool IsEmpty(){
            return isEmpty;
        }

        /// <summary>
        /// If a value is present, calls the given <see cref="Action{T}"/>
        /// with the value, otherwise does nothing.
        /// </summary>
        /// <param name="action">The action to call if a value is present</param>
        public void IfPresent(Action<T> action){
            if(IsPresent()){
                action(value);
            }
        }

        /// <summary>
        /// If a value is present, calls the given <see cref="Action{T}"/>
        /// with the value, otherwise calls the given <see cref="Action"/>.
        /// </summary>
        /// <param name="action">The action to call if a value is present</param>
        /// <param name="emptyAction">The action to call if this optional
        /// is empty</param>
        public void IfPresentOrElse(Action<T> action, Action emptyAction){
            if(IsPresent()){
                action(value);
            }else{
                emptyAction();
            }
        }

        /// <summary>
        /// If a value is present in this <see cref="Optional{T}"/> and that
        /// value matches the given <see cref="Predicate{T}"/>, returns an
        /// optional containing that value. Otherwise, returns an empty
        /// optional.
        /// </summary>
        /// <param name="predicate">The predicate to apply to the value</param>
        /// <returns>An optional containing the same value as this one, if a
        /// value is present and matches the given predicate, otherwise an
        /// empty optional</returns>
        public Optional<T> Filter(Predicate<T> predicate){
            if(predicate == null){
                throw new ArgumentNullException(nameof(predicate));
            }
            if(IsEmpty()){
                return this;
            }
            return predicate(value) ? this : Empty();
        }

        /// <summary>
        /// Returns an <see cref="Optional{T}"/> containing the result of
        /// applying the given <see cref="Func{T, TResult}"/> to the value in
        /// this optional, if present. If no value is present or if the given
        /// function returns <c>null</c>, an empty optional will be returned.
        /// </summary>
        /// <typeparam name="U">The type of the value returned by the given
        /// function</typeparam>
        /// <param name="mapper">The mapping function to apply to the value,
        /// if present</param>
        /// <returns>An optional containin the value returned by the mapping
        /// function if a value is present, otherwise an empty optional</returns>
        public Optional<U> Map<U>(Func<T, U> mapper){
            if(mapper == null){
                throw new ArgumentNullException(nameof(mapper));
            }
            if(IsEmpty()){
                return Optional<U>.Empty();
            }
            return Optional<U>.OfNullable(mapper(value));
        }

        /// <summary>
        /// Returns the result of applying the given <see cref="Func{T, TResult}"/>
        /// to the value in this optional, if present. If no value is present,
        /// an empty optional will be returned.
        /// </summary>
        /// <remarks>This method is similar to <see cref="Map{U}(Func{T, U})"/>,
        /// except that the mapping function is one which already returns an
        /// <see cref="Optional{T}"/>. When called, this function will not
        /// wrap the result in an additional optional.</remarks>
        /// <typeparam name="U">The type of the value returned by the given
        /// function</typeparam>
        /// <param name="mapper">The mapping function to apply to the value,
        /// if present</param>
        /// <returns>The optional returned by the mapping function if a value
        /// is present, otherwise an empty optional</returns>
        public Optional<U> FlatMap<U>(Func<T, Optional<U>> mapper){
            if(mapper == null){
                throw new ArgumentNullException(nameof(mapper));
            }
            if(IsEmpty()){
                return Optional<U>.Empty();
            }
            Optional<U> result = mapper(value);
            return result;
        }

        /// <summary>
        /// If a value is present in this <see cref="Optional{T}"/>, returns
        /// an optional containing that value. Otherwise, returns the optional
        /// returned by the given supplier function.
        /// </summary>
        /// <param name="supplier">A function which returns an optional to
        /// return if this optional is empty</param>
        /// <returns>An optional containing this optional's value if a value
        /// is present, otherwise the value returned by the given supplier
        /// function</returns>
        public Optional<T> Or(Func<Optional<T>> supplier){
            if(supplier == null){
                throw new ArgumentNullException(nameof(supplier));
            }
            if(IsPresent()){
                return this;
            }
            Optional<T> result = supplier();
            return result;
        }

        /// <summary>
        /// Returns the value in this <see cref="Optional{T}"/>, if present,
        /// otherwise returns <paramref name="other"/>.
        /// </summary>
        /// <param name="other">The value to return if this optional is empty</param>
        /// <returns>The value in this optional, if present, otherwise the
        /// given value</returns>
        public T OrElse(T other){
            return IsPresent() ? value : other;
        }

        /// <summary>
        /// Returns the value in this <see cref="Optional{T}"/> if one is
        /// present, otherwise returns the value returned by the given
        /// supplier <see cref="Func{T}"/>.
        /// </summary>
        /// <param name="supplier">The supplier function to invoke if no
        /// value is present</param>
        /// <returns>This optional's value, if present, otherwise the value
        /// returned by the given supplier</returns>
        public T OrElseGet(Func<T> supplier){
            return IsPresent() ? value : supplier();
        }

        /// <summary>
        /// Returns the value in this <see cref="Optional{T}"/> if present,
        /// otherwise throws an <see cref="InvalidOperationException"/>.
        /// </summary>
        /// <returns>The value contained in this optional, if present</returns>
        public T OrElseThrow(){
            if(IsEmpty()){
                throw new InvalidOperationException("No value present");
            }
            return value;
        }

        /// <summary>
        /// Returns the value in this <see cref="Optional{T}"/> if one is
        /// present, otherwise throws the <see cref="Exception"/> provided by
        /// the given supplier <see cref="Func{T}"/>.
        /// </summary>
        /// <typeparam name="X">The type of the exception to be thrown</typeparam>
        /// <param name="exceptionSupplier">The function that provides the
        /// exception to be thrown</param>
        /// <returns>The value contained in this optional, if present</returns>
        public T OrElseThrow<X>(Func<X> exceptionSupplier) where X : Exception{
            if(IsPresent()){
                return value;
            }
            throw exceptionSupplier();
        }


        /*
         * Override methods
         */
        /// <inheritdoc/>
        public override bool Equals(object obj){
            if(obj is Optional<T> other){
                return ObjectUtils.Equals(value, other.value) && isEmpty == other.isEmpty;
            }
            return false;
        }

        /// <summary>
        /// Gets the hash code of the value contained in this
        /// <see cref="Optional{T}"/>, if present. If no value is present,
        /// returns <c>0</c>.
        /// </summary>
        /// <returns>The hash code of this optional's value, if present,
        /// otherwise <c>0</c></returns>
        public override int GetHashCode(){
            return ObjectUtils.HashCode(value);
        }

        /// <summary>
        /// Gets a string representation of this <see cref="Optional{T}"/>
        /// containing the string representation of its value.
        /// </summary>
        /// <returns>A string representation of this optional</returns>
        public override string ToString(){
            if(IsEmpty()){
                return $"Optional<{typeof(T).Name}>.empty";
            }
            return $"Optional<{typeof(T).Name}>[{value}]";
        }


        /*
         * IEnumerable implementation
         */
        /// <summary>
        /// Gets an <see cref="IEnumerator{T}"/> containing the value in this
        /// <see cref="Optional{T}"/>, if present. Otherwise, returns an empty
        /// enumerator.
        /// </summary>
        /// <returns>An enumerator containing the value in this optional,
        /// if present</returns>
        public IEnumerator<T> GetEnumerator(){
            if(IsPresent()){
                yield return value;
            }
        }
        
        IEnumerator IEnumerable.GetEnumerator(){
            return GetEnumerator();
        }
    }
}
