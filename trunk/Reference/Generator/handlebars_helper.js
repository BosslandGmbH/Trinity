/* Handlebars Helper v0.0.11
   Generated on 2014-04-03 at 17:08:03 */

if (!String.prototype.contains) {
    String.prototype.contains = function () {
        return String.prototype.indexOf.apply(this, arguments) !== -1;
    };
}

function toTitleCase(str) {
    return str.replace(/\w\S*/g, function (txt) { return txt.charAt(0).toUpperCase() + txt.substr(1).toLowerCase(); });
}

matchString = 'best';
// If the match string is coming from user input you could do
// matchString = userInput.toLowerCase() here.

if (!String.prototype.icontains) {
    String.prototype.icontains = function () {
        return String.prototype.indexOf.apply(this.toLowerCase(), arguments) !== -1;
    };
}

(function ($) {
    $.extend({
        // Case insensative $.inArray (http://api.jquery.com/jquery.inarray/)
        // $.inArrayIn(value, array [, fromIndex])
        //  value (type: String)
        //    The value to search for
        //  array (type: Array)
        //    An array through which to search.
        //  fromIndex (type: Number)
        //    The index of the array at which to begin the search.
        //    The default is 0, which will search the whole array.
        inArrayIn: function (elem, arr, i) {
            // not looking for a string anyways, use default method
            if (typeof elem !== 'string') {
                return $.inArray.apply(this, arguments);
            }
            // confirm array is populated
            if (arr) {
                var len = arr.length;
                i = i ? (i < 0 ? Math.max(0, len + i) : i) : 0;
                elem = elem.toLowerCase();
                for (; i < len; i++) {
                    if (i in arr && arr[i].toLowerCase() == elem) {
                        return i;
                    }
                }
            }
            // stick with inArray/indexOf and return -1 on no match
            return -1;
        }
    });
})(jQuery);

!function (a) { if ("function" == typeof bootstrap) bootstrap("handlebars_helper", a); else if ("object" == typeof exports) module.exports = a(); else if ("function" == typeof define && define.amd) define(a); else if ("undefined" != typeof ses) { if (!ses.ok()) return; ses.makeHandlebarshelper = a } else "undefined" != typeof window ? window.handlebarshelper = a() : global.handlebarshelper = a() }(function () { return function a(b, c, d) { function e(g, h) { if (!c[g]) { if (!b[g]) { var i = "function" == typeof require && require; if (!h && i) return i(g, !0); if (f) return f(g, !0); throw new Error("Cannot find module '" + g + "'") } var j = c[g] = { exports: {} }; b[g][0].call(j.exports, function (a) { var c = b[g][1][a]; return e(c ? c : a) }, j, j.exports, a, b, c, d) } return c[g].exports } for (var f = "function" == typeof require && require, g = 0; g < d.length; g++) e(d[g]); return e }({ 1: [function (a, b) { b.exports = a("./lib") }, { "./lib": 26 }], 2: [function (a, b) { b.exports = function () { for (var a = Array.prototype.slice.call(arguments, 0, -1), b = 0, c = a.length - 1; c >= 0; c--) b += parseFloat(a[c], 10) || 0; return b } }, {}], 3: [function (a, b) { var c = a("new-date"), d = 31536e3, e = 2592e3, f = 86400, g = 3600; b.exports = function (a) { a = c(a); var b = Math.floor((new Date - a) / 1e3), h = Math.floor(b / d); return h > 1 ? h + " years ago" : (h = Math.floor(b / e), h > 1 ? h + " months ago" : (h = Math.floor(b / f), h > 1 ? h + " days ago" : (h = Math.floor(b / g), h > 1 ? h + " hours ago" : (h = Math.floor(b / 60), h > 1 ? h + " minutes ago" : Math.floor(b) <= 1 ? "Just now" : Math.floor(b) + " seconds ago")))) } }, { "new-date": 93 }], 4: [function (a, b) { b.exports = function (a, b, c, d) { if (d = d || c, "number" == typeof b) { c = "number" != typeof c ? a.length : c > 0 ? c + 1 : c, a = a.slice(b, c); for (var e = "", f = 0; f < a.length; f++) e += d.fn(a[f]); return e } } }, {}], 5: [function (a, b) { b.exports = function (a, b, c) { if ("string" == typeof a) return a.search(b) >= 0 ? c.fn(this) : c.inverse(this); for (var d in a) if (a.hasOwnProperty(d) && a[d] == b) return c.fn(this); return c.inverse(this) } }, {}], 6: [function (a, b) { b.exports = function () { for (var a = Array.prototype.slice.call(arguments, 1, -1), b = arguments[0], c = a.length - 1; c >= 0; c--) b /= parseFloat(a[c], 10) || 0; return b } }, {}], 7: [function (a, b) { b.exports = function (a) { return encodeURIComponent(a) } }, {}], 8: [function (a, b) { b.exports = function (a, b, c, d) { d = d || c, c = "exact" === c ? !0 : !1; var e = c ? a === b : a == b; return e ? d.fn(this) : d.inverse(this) } }, {}], 9: [function (a, b) { b.exports = function (a, b, c) { c = c || b, b = "number" == typeof b ? b : 1; var d = "", e = 0; for (var f in a) if (a.hasOwnProperty(f) && (d += c.fn(a[f]), e++, e == b)) break; return d } }, {}], 10: [function (a, b) { var c = a("strftime").strftimeTZ, d = a("new-date"); b.exports = function (a, b, e) { e = "number" == typeof e ? e : null; var f = d(a); return c(b, f, e) } }, { "new-date": 93, strftime: 98 }], 11: [function (a, b) { b.exports = function (a, b, c, d) { d = d || c, c = "equal" === c ? !0 : !1; var e = c ? a >= b : a > b; return e ? d.fn(this) : d.inverse(this) } }, {}], 12: [function (a, b) { b.exports = function (a, b) { if (b = "string" == typeof b ? b : "", a.join) return a.join(b); var c = ""; for (var d in a) a.hasOwnProperty(d) && (c += a[d] + b); return c.slice(0, -b.length) } }, {}], 13: [function (a, b) { var c = a("lodash.isarray"), d = a("lodash.reduce"); b.exports = function (a, b, e) { if (e = e || b, b = "number" == typeof b ? b : 1, !c(a)) { var f = []; for (var g in a) a.hasOwnProperty(g) && f.push(a[g]); a = f } var h = a.slice(-1 * b), i = d(h, function (a, b) { return a + e.fn(b) }, ""); return i } }, { "lodash.isarray": 27, "lodash.reduce": 29 }], 14: [function (a, b) { b.exports = function (a) { if (a.length) return a.length; var b = 0; for (var c in a) a.hasOwnProperty(c) && b++; return b } }, {}], 15: [function (a, b) { b.exports = function (a, b, c, d) { d = d || c, c = "equal" === c ? !0 : !1; var e = c ? b >= a : b > a; return e ? d.fn(this) : d.inverse(this) } }, {}], 16: [function (a, b) { b.exports = function (a) { return (a || "").toLowerCase() } }, {}], 17: [function (a, b) { b.exports = function () { for (var a = Array.prototype.slice.call(arguments, 1, -1), b = arguments[0], c = a.length - 1; c >= 0; c--) b *= parseFloat(a[c], 10) || 0; return b } }, {}], 18: [function (a, b) { b.exports = function (a, b, c, d) { if (d = d || c, "number" == typeof b) { var e = "number" != typeof c ? a.length : b + c; a = a.slice(b, e); for (var f = "", g = 0; g < a.length; g++) f += d.fn(a[g]); return f } } }, {}], 19: [function (a, b) { b.exports = function (a, b, c) { return (a || "").replace(b, c) } }, {}], 20: [function (a, b) { b.exports = function (a, b) { for (var c = "", d = a.length - 1; d >= 0; d--) c += b.fn(a[d]); return c } }, {}], 21: [function (a, b) { var c = function (a) { for (var b, c, d = a.length; d;) b = Math.floor(Math.random() * d--), c = a[d], a[d] = a[b], a[b] = c; return a }; b.exports = function (a, b) { for (var d = c(a), e = "", f = 0; f < d.length; f++) e += b.fn(d[f]); return e } }, {}], 22: [function (a, b) { b.exports = function () { for (var a = Array.prototype.slice.call(arguments, 1, -1), b = parseFloat(arguments[0], 10), c = a.length - 1; c >= 0; c--) b -= parseFloat(a[c], 10) || 0; return b } }, {}], 23: [function (a, b) { b.exports = function (a, b, c) { c = c || b, b = "zero" === b ? !0 : !1; var d, e = ""; if (b) for (d = 0; a > d; d++) e += c.fn(d); else for (d = 1; a >= d; d++) e += c.fn(d); return e } }, {}], 24: [function (a, b) { b.exports = function (a) { return (a || "").toUpperCase() } }, {}], 25: [function (a, b) { b.exports = function (a, b, c, d, e) { e = e || d, "number" != typeof d && (d = 1 / 0); for (var f = 0, g = "", h = 0; h < a.length; h++) if (a[h][b] === c && (g += e.fn(a[h]), f++, f === d)) return g; return g } }, {}], 26: [function (a, b) { var c = { lowercase: a("./helpers/lowercase.js"), uppercase: a("./helpers/uppercase.js"), replace: a("./helpers/replace.js"), encode: a("./helpers/encode.js"), length: a("./helpers/length.js"), contains: a("./helpers/contains.js"), first: a("./helpers/first.js"), last: a("./helpers/last.js"), between: a("./helpers/between.js"), range: a("./helpers/range.js"), where: a("./helpers/where.js"), shuffle: a("./helpers/shuffle.js"), reverse: a("./helpers/reverse.js"), join: a("./helpers/join.js"), ago: a("./helpers/ago.js"), formatDate: a("./helpers/formatDate.js"), equal: a("./helpers/equal.js"), greater: a("./helpers/greater.js"), less: a("./helpers/less.js"), times: a("./helpers/times.js"), add: a("./helpers/add.js"), subtract: a("./helpers/subtract.js"), multiply: a("./helpers/multiply.js"), divide: a("./helpers/divide.js") }; b.exports.help = function (a) { for (var b in c) a.registerHelper(b, c[b]) } }, { "./helpers/add.js": 2, "./helpers/ago.js": 3, "./helpers/between.js": 4, "./helpers/contains.js": 5, "./helpers/divide.js": 6, "./helpers/encode.js": 7, "./helpers/equal.js": 8, "./helpers/first.js": 9, "./helpers/formatDate.js": 10, "./helpers/greater.js": 11, "./helpers/join.js": 12, "./helpers/last.js": 13, "./helpers/length.js": 14, "./helpers/less.js": 15, "./helpers/lowercase.js": 16, "./helpers/multiply.js": 17, "./helpers/range.js": 18, "./helpers/replace.js": 19, "./helpers/reverse.js": 20, "./helpers/shuffle.js": 21, "./helpers/subtract.js": 22, "./helpers/times.js": 23, "./helpers/uppercase.js": 24, "./helpers/where.js": 25 }], 27: [function (a, b) { var c = a("lodash._isnative"), d = "[object Array]", e = Object.prototype, f = e.toString, g = c(g = Array.isArray) && g, h = g || function (a) { return a && "object" == typeof a && "number" == typeof a.length && f.call(a) == d || !1 }; b.exports = h }, { "lodash._isnative": 28 }], 28: [function (a, b) { function c(a) { return "function" == typeof a && f.test(a) } var d = Object.prototype, e = d.toString, f = RegExp("^" + String(e).replace(/[.*+?^${}()|[\]\\]/g, "\\$&").replace(/toString| for [^\]]+/g, ".*?") + "$"); b.exports = c }, {}], 29: [function (a, b) { function c(a, b, c, f) { if (!a) return c; var g = arguments.length < 3; b = d(b, f, 4); var h = -1, i = a.length; if ("number" == typeof i) for (g && (c = a[++h]) ; ++h < i;) c = b(c, a[h], h, a); else e(a, function (a, d, e) { c = g ? (g = !1, a) : b(c, a, d, e) }); return c } var d = a("lodash.createcallback"), e = a("lodash.forown"); b.exports = c }, { "lodash.createcallback": 30, "lodash.forown": 66 }], 30: [function (a, b) { function c(a, b, c) { var i = typeof a; if (null == a || "function" == i) return d(a, b, c); if ("object" != i) return h(a); var j = g(a), k = j[0], l = a[k]; return 1 != j.length || l !== l || f(l) ? function (b) { for (var c = j.length, d = !1; c-- && (d = e(b[j[c]], a[j[c]], null, !0)) ;); return d } : function (a) { var b = a[k]; return l === b && (0 !== l || 1 / l == 1 / b) } } var d = a("lodash._basecreatecallback"), e = a("lodash._baseisequal"), f = a("lodash.isobject"), g = a("lodash.keys"), h = a("lodash.property"); b.exports = c }, { "lodash._basecreatecallback": 31, "lodash._baseisequal": 50, "lodash.isobject": 59, "lodash.keys": 61, "lodash.property": 65 }], 31: [function (a, b) { function c(a, b, c) { if ("function" != typeof a) return e; if ("undefined" == typeof b || !("prototype" in a)) return a; var k = a.__bindData__; if ("undefined" == typeof k && (g.funcNames && (k = !a.name), k = k || !g.funcDecomp, !k)) { var l = j.call(a); g.funcNames || (k = !h.test(l)), k || (k = i.test(l), f(a, k)) } if (k === !1 || k !== !0 && 1 & k[1]) return a; switch (c) { case 1: return function (c) { return a.call(b, c) }; case 2: return function (c, d) { return a.call(b, c, d) }; case 3: return function (c, d, e) { return a.call(b, c, d, e) }; case 4: return function (c, d, e, f) { return a.call(b, c, d, e, f) } } return d(a, b) } var d = a("lodash.bind"), e = a("lodash.identity"), f = a("lodash._setbinddata"), g = a("lodash.support"), h = /^\s*function[ \n\r\t]+\w/, i = /\bthis\b/, j = Function.prototype.toString; b.exports = c }, { "lodash._setbinddata": 32, "lodash.bind": 35, "lodash.identity": 47, "lodash.support": 48 }], 32: [function (a, b) { var c = a("lodash._isnative"), d = a("lodash.noop"), e = { configurable: !1, enumerable: !1, value: null, writable: !1 }, f = function () { try { var a = {}, b = c(b = Object.defineProperty) && b, d = b(a, a, a) && b } catch (e) { } return d }(), g = f ? function (a, b) { e.value = b, f(a, "__bindData__", e) } : d; b.exports = g }, { "lodash._isnative": 33, "lodash.noop": 34 }], 33: [function (a, b) { b.exports = a(28) }, {}], 34: [function (a, b) { function c() { } b.exports = c }, {}], 35: [function (a, b) { function c(a, b) { return arguments.length > 2 ? d(a, 17, e(arguments, 2), null, b) : d(a, 1, null, null, b) } var d = a("lodash._createwrapper"), e = a("lodash._slice"); b.exports = c }, { "lodash._createwrapper": 36, "lodash._slice": 46 }], 36: [function (a, b) { function c(a, b, h, k, l, m) { var n = 1 & b, o = 2 & b, p = 4 & b, q = 16 & b, r = 32 & b; if (!o && !f(a)) throw new TypeError; q && !h.length && (b &= -17, q = h = !1), r && !k.length && (b &= -33, r = k = !1); var s = a && a.__bindData__; if (s && s !== !0) return s = g(s), s[2] && (s[2] = g(s[2])), s[3] && (s[3] = g(s[3])), !n || 1 & s[1] || (s[4] = l), !n && 1 & s[1] && (b |= 8), !p || 4 & s[1] || (s[5] = m), q && i.apply(s[2] || (s[2] = []), h), r && j.apply(s[3] || (s[3] = []), k), s[1] |= b, c.apply(null, s); var t = 1 == b || 17 === b ? d : e; return t([a, b, h, k, l, m]) } var d = a("lodash._basebind"), e = a("lodash._basecreatewrapper"), f = a("lodash.isfunction"), g = a("lodash._slice"), h = [], i = h.push, j = h.unshift; b.exports = c }, { "lodash._basebind": 37, "lodash._basecreatewrapper": 41, "lodash._slice": 46, "lodash.isfunction": 45 }], 37: [function (a, b) { function c(a) { function b() { if (h) { var a = g(h); i.apply(a, arguments) } if (this instanceof b) { var f = d(c.prototype), k = c.apply(f, a || arguments); return e(k) ? k : f } return c.apply(j, a || arguments) } var c = a[0], h = a[2], j = a[4]; return f(b, a), b } var d = a("lodash._basecreate"), e = a("lodash.isobject"), f = a("lodash._setbinddata"), g = a("lodash._slice"), h = [], i = h.push; b.exports = c }, { "lodash._basecreate": 38, "lodash._setbinddata": 32, "lodash._slice": 46, "lodash.isobject": 59 }], 38: [function (a, b) { function c(a) { return f(a) ? g(a) : {} } var d = "undefined" != typeof self ? self : "undefined" != typeof window ? window : {}, e = a("lodash._isnative"), f = a("lodash.isobject"); a("lodash.noop"); var g = e(g = Object.create) && g; g || (c = function () { function a() { } return function (b) { if (f(b)) { a.prototype = b; var c = new a; a.prototype = null } return c || d.Object() } }()), b.exports = c }, { "lodash._isnative": 39, "lodash.isobject": 59, "lodash.noop": 40 }], 39: [function (a, b) { b.exports = a(28) }, {}], 40: [function (a, b) { b.exports = a(34) }, {}], 41: [function (a, b) { function c(a) { function b() { var a = o ? m : this; if (k) { var f = g(k); i.apply(f, arguments) } if ((l || q) && (f || (f = g(arguments)), l && i.apply(f, l), q && f.length < n)) return j |= 16, c([h, r ? j : -4 & j, f, null, m, n]); if (f || (f = arguments), p && (h = a[s]), this instanceof b) { a = d(h.prototype); var t = h.apply(a, f); return e(t) ? t : a } return h.apply(a, f) } var h = a[0], j = a[1], k = a[2], l = a[3], m = a[4], n = a[5], o = 1 & j, p = 2 & j, q = 4 & j, r = 8 & j, s = h; return f(b, a), b } var d = a("lodash._basecreate"), e = a("lodash.isobject"), f = a("lodash._setbinddata"), g = a("lodash._slice"), h = [], i = h.push; b.exports = c }, { "lodash._basecreate": 42, "lodash._setbinddata": 32, "lodash._slice": 46, "lodash.isobject": 59 }], 42: [function (a, b) { b.exports = a(38) }, { "lodash._isnative": 43, "lodash.isobject": 59, "lodash.noop": 44 }], 43: [function (a, b) { b.exports = a(28) }, {}], 44: [function (a, b) { b.exports = a(34) }, {}], 45: [function (a, b) { function c(a) { return "function" == typeof a } b.exports = c }, {}], 46: [function (a, b) { function c(a, b, c) { b || (b = 0), "undefined" == typeof c && (c = a ? a.length : 0); for (var d = -1, e = c - b || 0, f = Array(0 > e ? 0 : e) ; ++d < e;) f[d] = a[b + d]; return f } b.exports = c }, {}], 47: [function (a, b) { function c(a) { return a } b.exports = c }, {}], 48: [function (a, b) { var c = "undefined" != typeof self ? self : "undefined" != typeof window ? window : {}, d = a("lodash._isnative"), e = /\bthis\b/, f = {}; f.funcDecomp = !d(c.WinRTError) && e.test(function () { return this }), f.funcNames = "string" == typeof Function.name, b.exports = f }, { "lodash._isnative": 49 }], 49: [function (a, b) { b.exports = a(28) }, {}], 50: [function (a, b) { function c(a, b, q, t, u, v) { if (q) { var w = q(a, b); if ("undefined" != typeof w) return !!w } if (a === b) return 0 !== a || 1 / a == 1 / b; var x = typeof a, y = typeof b; if (!(a !== a || a && g[x] || b && g[y])) return !1; if (null == a || null == b) return a === b; var z = r.call(a), A = r.call(b); if (z == i && (z = n), A == i && (A = n), z != A) return !1; switch (z) { case k: case l: return +a == +b; case m: return a != +a ? b != +b : 0 == a ? 1 / a == 1 / b : a == +b; case o: case p: return a == String(b) } var B = z == j; if (!B) { var C = s.call(a, "__wrapped__"), D = s.call(b, "__wrapped__"); if (C || D) return c(C ? a.__wrapped__ : a, D ? b.__wrapped__ : b, q, t, u, v); if (z != n) return !1; var E = a.constructor, F = b.constructor; if (E != F && !(f(E) && E instanceof E && f(F) && F instanceof F) && "constructor" in a && "constructor" in b) return !1 } var G = !u; u || (u = e()), v || (v = e()); for (var H = u.length; H--;) if (u[H] == a) return v[H] == b; var I = 0; if (w = !0, u.push(a), v.push(b), B) { if (H = a.length, I = b.length, w = I == H, w || t) for (; I--;) { var J = H, K = b[I]; if (t) for (; J-- && !(w = c(a[J], K, q, t, u, v)) ;); else if (!(w = c(a[I], K, q, t, u, v))) break } } else d(b, function (b, d, e) { return s.call(e, d) ? (I++, w = s.call(a, d) && c(a[d], b, q, t, u, v)) : void 0 }), w && !t && d(a, function (a, b, c) { return s.call(c, b) ? w = --I > -1 : void 0 }); return u.pop(), v.pop(), G && (h(u), h(v)), w } var d = a("lodash.forin"), e = a("lodash._getarray"), f = a("lodash.isfunction"), g = a("lodash._objecttypes"), h = a("lodash._releasearray"), i = "[object Arguments]", j = "[object Array]", k = "[object Boolean]", l = "[object Date]", m = "[object Number]", n = "[object Object]", o = "[object RegExp]", p = "[object String]", q = Object.prototype, r = q.toString, s = q.hasOwnProperty; b.exports = c }, { "lodash._getarray": 51, "lodash._objecttypes": 53, "lodash._releasearray": 54, "lodash.forin": 57, "lodash.isfunction": 58 }], 51: [function (a, b) { function c() { return d.pop() || [] } var d = a("lodash._arraypool"); b.exports = c }, { "lodash._arraypool": 52 }], 52: [function (a, b) { var c = []; b.exports = c }, {}], 53: [function (a, b) { var c = { "boolean": !1, "function": !0, object: !0, number: !1, string: !1, undefined: !1 }; b.exports = c }, {}], 54: [function (a, b) { function c(a) { a.length = 0, d.length < e && d.push(a) } var d = a("lodash._arraypool"), e = a("lodash._maxpoolsize"); b.exports = c }, { "lodash._arraypool": 55, "lodash._maxpoolsize": 56 }], 55: [function (a, b) { b.exports = a(52) }, {}], 56: [function (a, b) { var c = 40; b.exports = c }, {}], 57: [function (a, b) { var c = a("lodash._basecreatecallback"), d = a("lodash._objecttypes"), e = function (a, b, e) { var f, g = a, h = g; if (!g) return h; if (!d[typeof g]) return h; b = b && "undefined" == typeof e ? b : c(b, e, 3); for (f in g) if (b(g[f], f, a) === !1) return h; return h }; b.exports = e }, { "lodash._basecreatecallback": 31, "lodash._objecttypes": 53 }], 58: [function (a, b) { b.exports = a(45) }, {}], 59: [function (a, b) { function c(a) { return !(!a || !d[typeof a]) } var d = a("lodash._objecttypes"); b.exports = c }, { "lodash._objecttypes": 60 }], 60: [function (a, b) { b.exports = a(53) }, {}], 61: [function (a, b) { var c = a("lodash._isnative"), d = a("lodash.isobject"), e = a("lodash._shimkeys"), f = c(f = Object.keys) && f, g = f ? function (a) { return d(a) ? f(a) : [] } : e; b.exports = g }, { "lodash._isnative": 62, "lodash._shimkeys": 63, "lodash.isobject": 59 }], 62: [function (a, b) { b.exports = a(28) }, {}], 63: [function (a, b) { var c = a("lodash._objecttypes"), d = Object.prototype, e = d.hasOwnProperty, f = function (a) { var b, d = a, f = []; if (!d) return f; if (!c[typeof a]) return f; for (b in d) e.call(d, b) && f.push(b); return f }; b.exports = f }, { "lodash._objecttypes": 64 }], 64: [function (a, b) { b.exports = a(53) }, {}], 65: [function (a, b) { function c(a) { return function (b) { return b[a] } } b.exports = c }, {}], 66: [function (a, b) { var c = a("lodash._basecreatecallback"), d = a("lodash.keys"), e = a("lodash._objecttypes"), f = function (a, b, f) { var g, h = a, i = h; if (!h) return i; if (!e[typeof h]) return i; b = b && "undefined" == typeof f ? b : c(b, f, 3); for (var j = -1, k = e[typeof h] && d(h), l = k ? k.length : 0; ++j < l;) if (g = k[j], b(h[g], g, a) === !1) return i; return i }; b.exports = f }, { "lodash._basecreatecallback": 67, "lodash._objecttypes": 88, "lodash.keys": 89 }], 67: [function (a, b) { b.exports = a(31) }, { "lodash._setbinddata": 68, "lodash.bind": 71, "lodash.identity": 85, "lodash.support": 86 }], 68: [function (a, b) { b.exports = a(32) }, { "lodash._isnative": 69, "lodash.noop": 70 }], 69: [function (a, b) { b.exports = a(28) }, {}], 70: [function (a, b) { b.exports = a(34) }, {}], 71: [function (a, b) { b.exports = a(35) }, { "lodash._createwrapper": 72, "lodash._slice": 84 }], 72: [function (a, b) { b.exports = a(36) }, { "lodash._basebind": 73, "lodash._basecreatewrapper": 78, "lodash._slice": 84, "lodash.isfunction": 83 }], 73: [function (a, b) { b.exports = a(37) }, { "lodash._basecreate": 74, "lodash._setbinddata": 68, "lodash._slice": 84, "lodash.isobject": 77 }], 74: [function (a, b) { b.exports = a(38) }, { "lodash._isnative": 75, "lodash.isobject": 77, "lodash.noop": 76 }], 75: [function (a, b) { b.exports = a(28) }, {}], 76: [function (a, b) { b.exports = a(34) }, {}], 77: [function (a, b) { b.exports = a(59) }, { "lodash._objecttypes": 88 }], 78: [function (a, b) { b.exports = a(41) }, { "lodash._basecreate": 79, "lodash._setbinddata": 68, "lodash._slice": 84, "lodash.isobject": 82 }], 79: [function (a, b) { b.exports = a(38) }, { "lodash._isnative": 80, "lodash.isobject": 82, "lodash.noop": 81 }], 80: [function (a, b) { b.exports = a(28) }, {}], 81: [function (a, b) { b.exports = a(34) }, {}], 82: [function (a, b) { b.exports = a(59) }, { "lodash._objecttypes": 88 }], 83: [function (a, b) { b.exports = a(45) }, {}], 84: [function (a, b) { b.exports = a(46) }, {}], 85: [function (a, b) { b.exports = a(47) }, {}], 86: [function (a, b) { b.exports = a(48) }, { "lodash._isnative": 87 }], 87: [function (a, b) { b.exports = a(28) }, {}], 88: [function (a, b) { b.exports = a(53) }, {}], 89: [function (a, b) { b.exports = a(61) }, { "lodash._isnative": 90, "lodash._shimkeys": 91, "lodash.isobject": 92 }], 90: [function (a, b) { b.exports = a(28) }, {}], 91: [function (a, b) { b.exports = a(63) }, { "lodash._objecttypes": 88 }], 92: [function (a, b) { b.exports = a(59) }, { "lodash._objecttypes": 88 }], 93: [function (a, b) { function c(a) { return 315576e5 > a ? 1e3 * a : a } var d = a("is"), e = a("isodate"), f = a("./milliseconds"), g = a("./seconds"); b.exports = function (a) { return d.number(a) ? new Date(c(a)) : d.date(a) ? new Date(a.getTime()) : e.is(a) ? e.parse(a) : f.is(a) ? f.parse(a) : g.is(a) ? g.parse(a) : new Date(a) } }, { "./milliseconds": 94, "./seconds": 95, is: 96, isodate: 97 }], 94: [function (a, b, c) { var d = /\d{13}/; c.is = function (a) { return d.test(a) }, c.parse = function (a) { return a = parseInt(a, 10), new Date(a) } }, {}], 95: [function (a, b, c) { var d = /\d{10}/; c.is = function (a) { return d.test(a) }, c.parse = function (a) { var b = 1e3 * parseInt(a, 10); return new Date(b) } }, {}], 96: [function (a, b) { var c = Object.prototype, d = c.hasOwnProperty, e = c.toString, f = function (a) { return a !== a }, g = { "boolean": 1, number: 1, string: 1, undefined: 1 }, h = b.exports = {}; h.a = h.type = function (a, b) { return typeof a === b }, h.defined = function (a) { return void 0 !== a }, h.empty = function (a) { var b, c = e.call(a); if ("[object Array]" === c || "[object Arguments]" === c) return 0 === a.length; if ("[object Object]" === c) { for (b in a) if (d.call(a, b)) return !1; return !0 } return "[object String]" === c ? "" === a : !1 }, h.equal = function (a, b) { var c, d = e.call(a); if (d !== e.call(b)) return !1; if ("[object Object]" === d) { for (c in a) if (!h.equal(a[c], b[c])) return !1; return !0 } if ("[object Array]" === d) { if (c = a.length, c !== b.length) return !1; for (; --c;) if (!h.equal(a[c], b[c])) return !1; return !0 } return "[object Function]" === d ? a.prototype === b.prototype : "[object Date]" === d ? a.getTime() === b.getTime() : a === b }, h.hosted = function (a, b) { var c = typeof b[a]; return "object" === c ? !!b[a] : !g[c] }, h.instance = h["instanceof"] = function (a, b) { return a instanceof b }, h["null"] = function (a) { return null === a }, h.undefined = function (a) { return void 0 === a }, h.arguments = function (a) { var b = "[object Arguments]" === e.call(a), c = !h.array(a) && h.arraylike(a) && h.object(a) && h.fn(a.callee); return b || c }, h.array = function (a) { return "[object Array]" === e.call(a) }, h.arguments.empty = function (a) { return h.arguments(a) && 0 === a.length }, h.array.empty = function (a) { return h.array(a) && 0 === a.length }, h.arraylike = function (a) { return !!a && !h.boolean(a) && d.call(a, "length") && isFinite(a.length) && h.number(a.length) && a.length >= 0 }, h.boolean = function (a) { return "[object Boolean]" === e.call(a) }, h["false"] = function (a) { return h.boolean(a) && (a === !1 || a.valueOf() === !1) }, h["true"] = function (a) { return h.boolean(a) && (a === !0 || a.valueOf() === !0) }, h.date = function (a) { return "[object Date]" === e.call(a) }, h.element = function (a) { return void 0 !== a && "undefined" != typeof HTMLElement && a instanceof HTMLElement && 1 === a.nodeType }, h.error = function (a) { return "[object Error]" === e.call(a) }, h.fn = h["function"] = function (a) { var b = "undefined" != typeof window && a === window.alert; return b || "[object Function]" === e.call(a) }, h.number = function (a) { return "[object Number]" === e.call(a) }, h.infinite = function (a) { return 1 / 0 === a || a === -1 / 0 }, h.decimal = function (a) { return h.number(a) && !f(a) && 0 !== a % 1 }, h.divisibleBy = function (a, b) { var c = h.infinite(a), d = h.infinite(b), e = h.number(a) && !f(a) && h.number(b) && !f(b) && 0 !== b; return c || d || e && 0 === a % b }, h.int = function (a) { return h.number(a) && !f(a) && 0 === a % 1 }, h.maximum = function (a, b) { if (f(a)) throw new TypeError("NaN is not a valid value"); if (!h.arraylike(b)) throw new TypeError("second argument must be array-like"); for (var c = b.length; --c >= 0;) if (a < b[c]) return !1; return !0 }, h.minimum = function (a, b) { if (f(a)) throw new TypeError("NaN is not a valid value"); if (!h.arraylike(b)) throw new TypeError("second argument must be array-like"); for (var c = b.length; --c >= 0;) if (a > b[c]) return !1; return !0 }, h.nan = function (a) { return !h.number(a) || a !== a }, h.even = function (a) { return h.infinite(a) || h.number(a) && a === a && 0 === a % 2 }, h.odd = function (a) { return h.infinite(a) || h.number(a) && a === a && 0 !== a % 2 }, h.ge = function (a, b) { if (f(a) || f(b)) throw new TypeError("NaN is not a valid value"); return !h.infinite(a) && !h.infinite(b) && a >= b }, h.gt = function (a, b) { if (f(a) || f(b)) throw new TypeError("NaN is not a valid value"); return !h.infinite(a) && !h.infinite(b) && a > b }, h.le = function (a, b) { if (f(a) || f(b)) throw new TypeError("NaN is not a valid value"); return !h.infinite(a) && !h.infinite(b) && b >= a }, h.lt = function (a, b) { if (f(a) || f(b)) throw new TypeError("NaN is not a valid value"); return !h.infinite(a) && !h.infinite(b) && b > a }, h.within = function (a, b, c) { if (f(a) || f(b) || f(c)) throw new TypeError("NaN is not a valid value"); if (!h.number(a) || !h.number(b) || !h.number(c)) throw new TypeError("all arguments must be numbers"); var d = h.infinite(a) || h.infinite(b) || h.infinite(c); return d || a >= b && c >= a }, h.object = function (a) { return a && "[object Object]" === e.call(a) }, h.hash = function (a) { return h.object(a) && a.constructor === Object && !a.nodeType && !a.setInterval }, h.regexp = function (a) { return "[object RegExp]" === e.call(a) }, h.string = function (a) { return "[object String]" === e.call(a) } }, {}], 97: [function (a, b, c) { var d = /^(\d{4})(?:-?(\d{2})(?:-?(\d{2}))?)?(?:([ T])(\d{2}):?(\d{2})(?::?(\d{2})(?:[,\.](\d{1,}))?)?(?:(Z)|([+\-])(\d{2})(?::?(\d{2}))?)?)?$/; c.parse = function (a) { var b = [1, 5, 6, 7, 8, 11, 12], c = d.exec(a), e = 0; if (!c) return new Date(a); for (var f, g = 0; f = b[g]; g++) c[f] = parseInt(c[f], 10) || 0; c[2] = parseInt(c[2], 10) || 1, c[3] = parseInt(c[3], 10) || 1, c[2]--, c[8] && (c[8] = (c[8] + "00").substring(0, 3)), " " == c[4] ? e = (new Date).getTimezoneOffset() : "Z" !== c[9] && c[10] && (e = 60 * c[11] + c[12], "+" == c[10] && (e = 0 - e)); var h = Date.UTC(c[1], c[2], c[3], c[5], c[6] + e, c[7], c[8]); return new Date(h) }, c.is = function (a, b) { return b && !1 === /^\d{4}-\d{2}-\d{2}/.test(a) ? !1 : d.test(a) } }, {}], 98: [function (a, b) { !function () { function a(a) { return (a || "").split(" ") } function c(a, b, c) { return g(a, b, c) } function d(a, b, c, d) { return "number" == typeof c && null == d && (d = c, c = void 0), g(a, b, c, { timezone: d }) } function e(a, b, c) { return g(a, b, c, { utc: !0 }) } function f(a) { return function (b, d, e) { return c(b, d, a, e) } } function g(a, b, c, d) { d = d || {}, b && !i(b) && (c = b, b = void 0), b = b || new Date, c = c || o, c.formats = c.formats || {}; var e = b.getTime(); return (d.utc || "number" == typeof d.timezone) && (b = h(b)), "number" == typeof d.timezone && (b = new Date(b.getTime() + 6e4 * d.timezone)), a.replace(/%([-_0]?.)/g, function (a, f) { var h, i; if (2 == f.length) { if (h = f[0], "-" == h) i = ""; else if ("_" == h) i = " "; else { if ("0" != h) return a; i = "0" } f = f[1] } switch (f) { case "A": return c.days[b.getDay()]; case "a": return c.shortDays[b.getDay()]; case "B": return c.months[b.getMonth()]; case "b": return c.shortMonths[b.getMonth()]; case "C": return j(Math.floor(b.getFullYear() / 100), i); case "D": return g(c.formats.D || "%m/%d/%y", b, c); case "d": return j(b.getDate(), i); case "e": return b.getDate(); case "F": return g(c.formats.F || "%Y-%m-%d", b, c); case "H": return j(b.getHours(), i); case "h": return c.shortMonths[b.getMonth()]; case "I": return j(k(b), i); case "j": var n = new Date(b.getFullYear(), 0, 1), o = Math.ceil((b.getTime() - n.getTime()) / 864e5); return j(o, 3); case "k": return j(b.getHours(), null == i ? " " : i); case "L": return j(Math.floor(e % 1e3), 3); case "l": return j(k(b), null == i ? " " : i); case "M": return j(b.getMinutes(), i); case "m": return j(b.getMonth() + 1, i); case "n": return "\n"; case "o": return String(b.getDate()) + l(b.getDate()); case "P": return b.getHours() < 12 ? c.am : c.pm; case "p": return b.getHours() < 12 ? c.AM : c.PM; case "R": return g(c.formats.R || "%H:%M", b, c); case "r": return g(c.formats.r || "%I:%M:%S %p", b, c); case "S": return j(b.getSeconds(), i); case "s": return Math.floor(e / 1e3); case "T": return g(c.formats.T || "%H:%M:%S", b, c); case "t": return "	"; case "U": return j(m(b, "sunday"), i); case "u": var o = b.getDay(); return 0 == o ? 7 : o; case "v": return g(c.formats.v || "%e-%b-%Y", b, c); case "W": return j(m(b, "monday"), i); case "w": return b.getDay(); case "Y": return b.getFullYear(); case "y": var n = String(b.getFullYear()); return n.slice(n.length - 2); case "Z": if (d.utc) return "GMT"; var p = b.toString().match(/\((\w+)\)/); return p && p[1] || ""; case "z": if (d.utc) return "+0000"; var q = "number" == typeof d.timezone ? d.timezone : -b.getTimezoneOffset(); return (0 > q ? "-" : "+") + j(Math.abs(q / 60)) + j(q % 60); default: return f } }) } function h(a) { var b = 6e4 * (a.getTimezoneOffset() || 0); return new Date(a.getTime() + b) } function i(a) { var b = 0, c = p.length; for (b = 0; c > b; ++b) if ("function" != typeof a[p[b]]) return !1; return !0 } function j(a, b, c) { "number" == typeof b && (c = b, b = "0"), null == b && (b = "0"), c = c || 2; var d = String(a); if (b) for (; d.length < c;) d = b + d; return d } function k(a) { var b = a.getHours(); return 0 == b ? b = 12 : b > 12 && (b -= 12), b } function l(a) { var b = a % 10, c = a % 100; if (c >= 11 && 13 >= c || 0 === b || b >= 4) return "th"; switch (b) { case 1: return "st"; case 2: return "nd"; case 3: return "rd" } } function m(a, b) { b = b || "sunday"; var c = a.getDay(); "monday" == b && (0 == c ? c = 6 : c--); var d = new Date(a.getFullYear(), 0, 1), e = (a - d) / 864e5, f = (e + 7 - c) / 7; return Math.floor(f) } var n; n = "undefined" != typeof b ? b.exports = c : function () { return this || (1, eval)("this") }(); var o = { days: a("Sunday Monday Tuesday Wednesday Thursday Friday Saturday"), shortDays: a("Sun Mon Tue Wed Thu Fri Sat"), months: a("January February March April May June July August September October November December"), shortMonths: a("Jan Feb Mar Apr May Jun Jul Aug Sep Oct Nov Dec"), AM: "AM", PM: "PM", am: "am", pm: "pm" }; n.strftime = c, n.strftimeTZ = c.strftimeTZ = d, n.strftimeUTC = c.strftimeUTC = e, n.localizedStrftime = c.localizedStrftime = f; var p = ["getTime", "getTimezoneOffset", "getDay", "getDate", "getMonth", "getFullYear", "getYear", "getHours", "getMinutes", "getSeconds"] }() }, {}] }, {}, [1])(1) });

function getMatches(string, regex, element) {    
    var validMatches = [];
    if (!string || !regex) validMatches;
    var matches = regex.exec(string);
    if (matches != null) {
        for (var i = 0; i < matches.length; i++) {
            if (matches[i]) validMatches.push(matches[i]);
        }        
    }
    if (validMatches && validMatches.length > 0) {
        if (element && element == "last") {
            return validMatches[validMatches.length - 1]
        }
        else if (element && element == "first") {
            return validMatches[0]
        }
        else if (typeof element === 'number') {
            return validMatches[element]
        }
        return validMatches;       
    }
    return null;
}

Handlebars.registerHelper('tolower', function (options) {
    return options.fn(this).toLowerCase();
});

Handlebars.registerHelper('Bool', function (options) {
    return (options.fn(this) == null || options.fn(this) == false || options.fn(this) == "false") ? "false" : "true";
});

// Convert a friendly name into something we can use as a class/property name
Handlebars.registerHelper('Strip', function (string) {

    return string.replace(/\w\S*/g, function (txt) { return txt.charAt(0).toUpperCase() + txt.substr(1).toLowerCase(); }).replace(/\W|\s/g,'');

});

var ToTitleCase = function (string) {
    return string.replace(/\w\S*/g, function (txt) { return txt.charAt(0).toUpperCase() + txt.substr(1).toLowerCase(); }).replace(/\W|\s/g,'');
}

// Convert a friendly name into something we can use as a class/property name
Handlebars.registerHelper('Bool', function (string) {    
    return (string && string.toString().toLowerCase() == "true") ? "true" : "false";
});

// Convert a friendly name into something we can use as a class/property name
Handlebars.registerHelper('StripRune', function (string, skill) {

    if (!skill) skill = "";

    var result = string.replace(/\w\S*/g, function (txt) { return txt.charAt(0).toUpperCase() + txt.substr(1).toLowerCase(); }).replace(/\W|\s/g, '');

    // Why would blizzard have two runes the same name?
    if (result == "ChargedUp") {
        result = result + skill.replace(/\w\S*/g, function (txt) { return txt.charAt(0).toUpperCase() + txt.substr(1).toLowerCase(); }).replace(/\W|\s/g, '');
    }

    return result;
});

// Convert a Type letter code to RuneIndex
Handlebars.registerHelper('RuneIndex', function (string) {
    switch (string) {
        case "a" : return 0
        case "b" : return 1
        case "c" : return 2
        case "d" : return 3
        case "e" : return 4
    }
    return "-1"
});

var SpecificCaseWords = [
    "DemonHunter",
    "Witchdoctor",
    "BulKathos",
    "BoonOfBulKathos",
    "ArcaneAegis",
    "CorpseSpider",
    "NephalemMajesty",
    "FistsofThunder",
    "SevenSidedStrike",
    "LawsOfHope2",
    "LawsOfJustice2",
    "LawsOfValor2",
    "HeavensFury3",
    "ShieldBash2",
    "SummonZombieDog",
    "v2",
    "X1"
];

var DontTouchWords = [
    //"x1"
];

Handlebars.registerHelper('Format', function (string) {

    var DontTouchWordsIndex = $.inArrayIn(string, DontTouchWords);
    var SpecificCaseWordsIndex = $.inArrayIn(string, SpecificCaseWords);

    // some tokens are case sensitive x1 / X1
    if (DontTouchWordsIndex >= 0) {
        return string;
    }

    // some tokens we have no way of deciphering the proper case
    else if (SpecificCaseWordsIndex >= 0) {
        return SpecificCaseWords[SpecificCaseWordsIndex];
    }

    // title case
    else {

        return string.replace(/\w\S*/g, function (txt) { return txt.charAt(0).toUpperCase() + txt.substr(1); });
    }

});



//Handlebars.registerHelper('ICombatProperties', function (description, type) {

//    var result = "";
//    var skillname = this.name.replace(/\w\S*/g, function (txt) { return txt.charAt(0).toUpperCase() + txt.substr(1).toLowerCase(); }).replace(/\W|\s/g, '');

//    switch (type) {

//        case "Duration":

//            var durationRegex = /for ([\d|.]+) second|lasts ([\d|.]+) seconds|to ([\d|.]+) seconds|over ([\d|.]+) seconds/gi;
//            var duration = getMatches(description, durationRegex, "last");
//            if (duration) {
//                result = "TimeSpan.FromSeconds(" + duration + ")";
//                break;
//            }

//            var durationMinutesRegex = /lasts ([\d|.]+) minutes/gi;
//            var durationMinutes = getMatches(description, durationMinutesRegex, "last");
//            if (durationMinutes) {
//                result = "TimeSpan.FromMinutes(" + durationMinutes + ")";
//                break;
//            }

//            result = "TimeSpan.Zero";
//            break;

//        case "Cost":

//            var costRegex = /cost: ([\d|.]+)|cost to ([\d|.]+)|costs ([\d|.]+)|cost of \w+ to ([\d|.]+)|cost of *([a-zA-Z]+\s*){1,3} to ([\d|.]+)|add a ([\d|.]+) \w+ cost/gi;
//            var cost = getMatches(description, costRegex, "last");

//            if (cost) {
//                result = cost;
//                break;
//            }

//            result = "0";

//            break;

//        case "Cooldown":

//            if (description.contains("Remove the cooldown")) {
//                result = "TimeSpan.Zero";
//                break;
//            }

//            var cooldownRegex = /cooldown of \w+ to ([\d|.]+) seconds|reduce the cooldown to ([\d|.]+)|([\d|.]+) second cooldown|Cooldown: ([\d|.]+) seconds|once per ([\d|.]+) seconds/gi;
//            var cooldown = getMatches(description, cooldownRegex, "last");

//            if (cooldown) {
//                result = "TimeSpan.FromSeconds(" + cooldown + ")";
//                break;
//            }

//            var cooldownMinutesRegex = /lasts ([\d|.]+) minutes/gi;
//            var cooldownMinutes = getMatches(description, cooldownMinutesRegex, "last");
//            if (cooldownMinutes) {
//                result = "TimeSpan.FromMinutes(" + cooldownMinutes + ")";
//                break;
//            }

//            result = "TimeSpan.Zero";
//            break;

//        //case "IsDot":

//        //    var isDotRegex = /over ([\d|.]+) seconds/gi;
//        //    var isDot = getMatches(description, isDotRegex, "last");
//        //    result = isDot ? "True" : "False";
 
//        //    break;

//        //case "IsGenerator":

//        //    var isGeneratorRegex = /generate: (\d+)/gi;
//        //    var isGenerator = $(isGeneratorRegex.exec(description)).last().get(0);
//        //    result = isDot ? "True" : "False";

//        //    break;

//        case "Resource":            

//            // not useful for runes - they wont mention resource type unless amount has changed.

//            if (description.icontains("signature spell")) {
//                result = "Element.None";
//                break;
//            }

//            var resourceRegex = /(Arcane)|(Mana)|(Spirit)|(Fury)|(Wrath)|(Discipline)|(Hatred)/gi;
//            var match = getMatches(description, resourceRegex, "last");
//            if (match) {
//                // title case
//                result = "Resource." + ToTitleCase(match);
//            } else {
//                // allow a none resource type since you cant assume demonhunter skill is discipline or hatred
//                result = "Resource.None";
//            }

//            break;

//        case "Element":

//            var resourceRegex = /as (Arcane)|as (Fire)|as (Cold)|as (Lightning)|as (Poison)|as (Holy)|as (Physical)|to (Arcane)|to (Fire)|to (Cold)|to (Lightning)|to (Poison)|to (Holy)|to (Physical)|deal (Arcane)|deal (Fire)|deal (Cold)|deal (Lightning)|deal (Poison)|deal (Holy)|deal (Physical)/gi;
//            var match = getMatches(description, resourceRegex, "last");
//            if (match) {
//                result = "Element." + ToTitleCase(match);
//            } else {

//                var found = false;
//                $(UnlistedSkillElements).each(function() {

//                    var element = this[1];

//                    if (this[0] == skillname) {
//                        result = "Element." + ToTitleCase(element);
//                        found = true;
//                    }

//                });
                
//                if (!found) {
//                    result = "Element.Unknown";
//                }

//            }

//            break;

//    }

//    return result;
//});


Handlebars.registerHelper('ICombatProperties', function (description, type) {

    var result = "";
    var skillname = this.name.replace(/\w\S*/g, function (txt) { return txt.charAt(0).toUpperCase() + txt.substr(1).toLowerCase(); }).replace(/\W|\s/g, '');

    var isRune = this.tooltipParams.icontains("rune/");
    var isSkill = this.tooltipParams.icontains("skill/");

    var prefix = (isRune ? "Modified" : "");

    // for runes we dont want to set a value if the rune isn't modifying it
    var setDefault = isSkill;

    switch (type) {

        case "Duration":

            var durationRegex = /for ([\d|.]+) second|lasts ([\d|.]+) seconds|to ([\d|.]+) seconds|over ([\d|.]+) seconds/gi;
            var duration = getMatches(description, durationRegex, "last");
            if (duration) {
                result = prefix + "Duration = TimeSpan.FromSeconds(" + duration + "),<br/>";
                break;
            }

            var durationMinutesRegex = /lasts ([\d|.]+) minutes/gi;
            var durationMinutes = getMatches(description, durationMinutesRegex, "last");
            if (durationMinutes) {
                result = prefix + "Duration = TimeSpan.FromMinutes(" + durationMinutes + "),<br/>";
                break;
            }

            if (setDefault) {
                result = prefix + "Duration = TimeSpan.Zero, <br/>";
            }

            break;

        case "Cost":

            var costRegex = /cost: ([\d|.]+)|cost to ([\d|.]\d+)|costs ([\d|.]+)|cost of \w+ to ([\d|.]+)|cost of *([a-zA-Z]+\s*){1,3} to ([\d|.]+)|add a ([\d|.]+) \w+ cost/gi;
            var cost = getMatches(description, costRegex, "last");

            if (cost) {
                result = prefix + "Cost = " + cost + ",<br/>";
                break;
            }

            if (setDefault) {
                result = prefix + "Cost = 0,<br/>";
            }

            break;

        case "Cooldown":

            if (description.contains("Remove the cooldown")) {
                result = prefix + "Cooldown = TimeSpan.Zero,<br/>";
                break;
            }

            var cooldownRegex = /cooldown of \w+ to ([\d|.]+) seconds|reduce the cooldown to ([\d|.]+)|([\d|.]+) second cooldown|Cooldown: ([\d|.]+) seconds|once per ([\d|.]+) seconds/gi;
            var cooldown = getMatches(description, cooldownRegex, "last");

            if (cooldown) {
                result = prefix + "Cooldown = TimeSpan.FromSeconds(" + cooldown + "),<br/>";
                break;
            }

            var cooldownMinutesRegex = /lasts ([\d|.]+) minutes/gi;
            var cooldownMinutes = getMatches(description, cooldownMinutesRegex, "last");
            if (cooldownMinutes) {
                result = prefix + "Cooldown = TimeSpan.FromMinutes(" + cooldownMinutes + "),<br/>";
                break;
            }

            if (setDefault) {
                result = prefix + "Cooldown = TimeSpan.Zero,<br/>";
            }

            break;

            //case "IsDot":

            //    var isDotRegex = /over ([\d|.]+) seconds/gi;
            //    var isDot = getMatches(description, isDotRegex, "last");
            //    result = isDot ? "True" : "False";

            //    break;

            //case "IsGenerator":

            //    var isGeneratorRegex = /generate: (\d+)/gi;
            //    var isGenerator = $(isGeneratorRegex.exec(description)).last().get(0);
            //    result = isDot ? "True" : "False";

            //    break;

        case "IsDamaging":
           
            var regex = /(deals)|(dealing)|(weapon damage)/gi;
            var match = getMatches(description, regex, "last");
            if (match) {
                result = prefix + "IsDamaging = true,<br/>";
            } else {
                if (setDefault) {
                    result = prefix + "IsDamaging = false,<br/>";
                }
            }
            break;

        case "AreaEffectRadius":
           
            var regex = /within ([\d|.]+) yards/gi;
            var match = getMatches(description, regex, "last");
            if (match) {
                result = prefix + "AreaEffectRadius = " + match + "f,<br/>";
            } else {

                var regex = /large area/gi;
                var match = getMatches(description, regex, "last");
                if (match) {
                    result = prefix + "AreaEffectRadius = 85f,<br/>";
                } else {
                    if (setDefault) {
                        result = prefix + "AreaEffectRadius = 0f,<br/>";
                    }
                }
            }
            break;

        case "ResourceEffect":            

            var costRegex = /cost: ([\d|.]+)|cost to ([\d|.]\d+)|costs ([\d|.]+)|cost of \w+ to ([\d|.]+)|cost of *([a-zA-Z]+\s*){1,3} to ([\d|.]+)|add a ([\d|.]+) \w+ cost/gi;
            var cost = getMatches(description, costRegex, "last");
            if (cost) {
                result = "ResourceEffect = ResourceEffectType.Spender,<br/>";
                break;
            }

            var generateRegex = /(generate)/gi;
            var match = getMatches(description, generateRegex, "last");
            if (match) {
                result = prefix + "ResourceEffect = ResourceEffectType.Generator,<br/>";
                break;
            }
            
                result = prefix + "ResourceEffect = ResourceEffectType.None,<br/>";
            
            break;

        case "SkillType":

            var costRegex = /cost: ([\d|.]+)|cost to ([\d|.]\d+)|costs ([\d|.]+)|cost of \w+ to ([\d|.]+)|cost of *([a-zA-Z]+\s*){1,3} to ([\d|.]+)|add a ([\d|.]+) \w+ cost/gi;
            var cost = getMatches(description, costRegex, "last");
            if (cost) {
                result = prefix + "SkillType = SkillType.Spender,<br/>";
                break;
            }

            var generateRegex = /(generate)/gi;
            var match = getMatches(description, generateRegex, "last");
            if (match) {
                result = prefix + "SkillType = SkillType.Generator,<br/>";
                break;
            }

            if (setDefault) {
                result = prefix + "SkillType = SkillType.None,<br/>";
            }
            break;

        case "Resource":

            // not useful for runes - they wont mention resource type unless amount has changed.

            if (description.icontains("signature spell")) {
                result = "Resource = Resource.None,<br/>";
                break;
            }

            var resourceRegex = /(Arcane)|(Mana)|(Spirit)|(Fury)|(Wrath)|(Discipline)|(Hatred)/gi;
            var match = getMatches(description, resourceRegex, "last");
            if (match) {
                result = prefix + "Resource = Resource." + ToTitleCase(match) + ",<br/>";
            } else {
                // allow a none resource type since you cant assume demonhunter skill is discipline or hatred
                if (setDefault) {
                    result = prefix + "Resource = Resource.None,<br/>";
                }

            }

            break;

        case "Element":

            var resourceRegex = /as (Arcane)|as (Fire)|as (Cold)|as (Lightning)|as (Poison)|as (Holy)|as (Physical)|to (Arcane)|to (Fire)|to (Cold)|to (Lightning)|to (Poison)|to (Holy)|to (Physical)|deal (Arcane)|deal (Fire)|deal (Cold)|deal (Lightning)|deal (Poison)|deal (Holy)|deal (Physical)/gi;
            var match = getMatches(description, resourceRegex, "last");
            if (match) {
                result = prefix + "Element = Element." + match + ",<br/>";
            } else {

                if (setDefault) {

                    var found = false;
                    $(UnlistedSkillElements).each(function () {

                        var element = this[1];

                        if (this[0] == skillname) {
                            result = prefix + "Element = Element." + ToTitleCase(element) + ",<br/>";
                            found = true;
                        }

                    });

                    if (!found) {
                        result = prefix + "Element = Element.Unknown,<br/>";
                    }
                    
                }
            }

            break;

    }

    return result;
});

// these skills dont mention their damage type on tooltip
var UnlistedSkillElements = [

    // Barbarian
    ["Bash", "Physical"],
    ["HammerOfTheAncients", "Physical"],
    ["Cleave", "Physical"],
    ["GroundStomp", "Physical"],
    ["Leap", "Physical"],
    ["Overpower", "Physical"],
    ["Frenzy", "Physical"],
    ["SeismicSlam", "Physical"],
    ["Revenge", "Physical"],
    ["ThreateningShout", "Physical"],
    ["Sprint", "Physical"],
    ["WeaponThrow", "Physical"],
    ["Whirlwind", "Physical"],
    ["FuriousCharge", "Physical"],
    ["IgnorePain", "Physical"],
    ["BattleRage", "Physical"],
    ["CallOfTheAncients", "Physical"],
    ["AncientSpear", "Physical"],
    ["WarCry", "Physical"],
    ["WrathOfTheBerserker", "Physical"],
    ["Avalanche", "Physical"],

    //Crusader
    ["Punish", "Physical"],
    ["ShieldGlare", "Holy"],
    ["SweepAttack", "Physical"],
    ["IronSkin", "Physical"],
    ["Provoke", "Physical"],
    ["SteedCharge", "Physical"],
    ["LawsOfValor", "Physical"],
    ["Justice", "Holy"],
    ["Consecration", "Holy"],
    ["LawsOfJustice", "Physical"],
    ["FallingSword", "Physical"],
    ["Judgment", "Physical"],
    ["LawsOfHope", "Physical"],
    ["AkaratsChampion", "Fire"],
    ["Phalanx", "Physical"],
    ["Bombardment", "Physical"],

    //Demonhunter
    ["HungeringArrow", "Physical"],
    ["Impale", "Physical"],
    ["EntanglingShot", "Physical"],
    ["Caltrops", "Physical"],
    ["SmokeScreen", "Physical"],
    ["Vault", "Physical"],
    ["Preparation", "Physical"],
    ["FanOfKnives", "Physical"],
    ["EvasiveFire", "Physical"],
    ["ShadowPower", "Physical"],
    ["Strafe", "Physical"],
    ["MarkedForDeath", "Physical"],
    ["Multishot", "Physical"],
    ["Sentry", "Physical"],
    ["RainOfVengeance", "Physical"],
    ["Vengeance", "Physical"],

    //Monk
    ["BlindingFlash", "Holy"],
    ["TempestRush", "Physical"],
    ["BreathOfHeaven", "Holy"],
    ["Serenity", "Holy"],
    ["SevensidedStrike", "Physical"],
    ["MantraOfEvasion", "Physical"],
    ["SweepingWind", "Physical"],
    ["InnerSanctuary", "Holy"],
    ["MantraOfHealing", "Holy"],
    ["MantraOfConviction", "Physical"],
    ["Epiphany", "Holy"],

    //Witchdoctor
    ["Horrify", "Physical"],
    ["SoulHarvest", "Physical"],
    ["SpiritWalk", "Physical"],
    ["Hex", "Physical"],
    ["MassConfusion", "Physical"],
    ["BigBadVoodoo", "Physical"],

    // Wizard
    ["FrostNova", "Cold"],
    ["DiamondSkin", "Arcane"],
    ["IceArmor", "Cold"],
    ["SlowTime", "Arcane"],
    ["MagicWeapon", "Arcane"],
    ["Teleport", "Arcane"],
    ["MirrorImage", "Arcane"],
    ["EnergyArmor", "Arcane"],
    ["Archon", "Arcane"],
]



Handlebars.registerHelper('UnknownElement', function (description, skill) {
    var resourceRegex = /as (Arcane)|as (Fire)|as (Cold)|as (Lightning)|as (Poison)|as (Holy)|as (Physical)|to (Arcane)|to (Fire)|to (Cold)|to (Lightning)|to (Poison)|to (Holy)|to (Physical)|deal (Arcane)|deal (Fire)|deal (Cold)|deal (Lightning)|deal (Poison)|deal (Holy)|deal (Physical)/gi;
    var match = getMatches(description, resourceRegex, "last");

    skill = skill.replace(/\w\S*/g, function (txt) { return txt.charAt(0).toUpperCase() + txt.substr(1).toLowerCase(); }).replace(/\W|\s/g, '');

    if (!match) {
        return result = "[\"" + skill + "\",\"Unknown\"],\n";
    }
});

Handlebars.registerHelper('SNOPowerFormat', function (string, name) {

    var parts = string.split("_");
    var stripName = name.replace(/\w\S*/g, function (txt) { return txt.charAt(0).toUpperCase() + txt.substr(1).toLowerCase(); }).replace(/\W|\s/g, '');
    
    $.each(parts, function(i, v) {

        var DontTouchWordsIndex = $.inArrayIn(v.toLowerCase(), DontTouchWords);
        var SpecificCaseWordsIndex = $.inArrayIn(v, SpecificCaseWords);
        var x1Exception = parts[i] == "x1" && (name == "Dominance" || name == "Phalanx");

        // some tokens are case sensitive x1 / X1
        if (DontTouchWordsIndex >= 0 || x1Exception) {
            parts[i] = v;
        }

        // some tokens we have no way of deciphering the proper case
        else if (SpecificCaseWordsIndex >= 0) {
            parts[i] = SpecificCaseWords[SpecificCaseWordsIndex];
        }

        // case rune/skill name based on the 'name' field
        else if (parts[i].toLowerCase() == stripName.toLowerCase()) {
            parts[i] = stripName;
        }

        // title case
        else {
            parts[i] = v.replace(/\w\S*/g, function (txt) { return txt.charAt(0).toUpperCase() + txt.substr(1); });
        }
    });

    return parts.join("_");
});
