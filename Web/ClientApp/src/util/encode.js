export function encodeText(text)
{
    return btoa(text);
}

export function decodeText(text)
{
    return atob(text);
}

export function encodeUnicode(text)
{
    return btoa(encodeURIComponent(text).replace(/%[0-9A-F]{2}/g,
        function toSolidBytes(match, p1)
        {
            return String.fromCharCode('0x' + p1);
        }));
}

export function decodeUnicode(text)
{
    return decodeURIComponent(atob(text).split('').map(function(c){
        return "%" + ("00" + c.charAt(0).toString(16)).slice(-2);
    }).join(""));
}

export default ({
    encodeText,
    decodeText,
    encodeUnicode,
    decodeUnicode
})