/**
 * ropufu.math.js
 *
 * Extras for the Math.js library.
 * Structure taken from https://stackoverflow.com/a/5947280.
 * 
 */
(function (ropufu, $, undefined) {
    // Use as return result for failed calls.
    /*private*/ const bad = {
        toString: function () {
            return "Something went wrong.";
        }
    };

    // Readonly object to test for failed non-numeric-valued returns.
    Object.defineProperty(ropufu, "bad", {
        value: bad,
        writable: false,
        configurable: false
    });

    // Accuracy target for quantile computations.
    /*public*/ ropufu.epsilon = 1e-5;

    // Quantiles of a distribution.
    /*private*/ function numericalQuantile(p, cdf, pdf, absoluteTolerance) {
        if (!math.isNumeric(p))
            return Number.NaN;
        if (p < 0 || p > 1)
            return Number.NaN;

        let dx = 1.0;
        let x = 0.0;
        while (math.abs(dx) > absoluteTolerance) {
            let y = x - (cdf(x) - p) / pdf(x);
            dx = y - x;
            x = y;
        }
        return x;
    };

    // Arcsine cdf(z), aka Beta cdf (z; 1/2, 1/2), aka regularized incomplete beta function I_z(1/2, 1/2).
    /*private*/ function arcsineCdf(z) {
        return 0.5 + math.asin(2 * z - 1) / math.PI;
    };

    // Regularized incomplete beta function I_z(n/2, 1/2), aka Beta cdf (z; n/2, 1/2).
    /*private*/ function arithmeticBetaCdf(z, n) {
        if (!math.isNumeric(z))
            return Number.NaN;
        if (z < 0 || z > 1)
            return Number.NaN;

        if (!math.isInteger(n))
            return Number.NaN;
        if (n <= 0)
            return Number.NaN;

        if (n % 2 == 0) {
            let k = n / 2;
            let sum = 0.0;
            // Note: math.gamma(1.5) == math.sqrt(math.PI) / 2.0.
            let nextTerm = z * math.sqrt(math.PI) / 2.0;
            for (let j = 1; j <= k - 1; ++j) {
                //sum += (math.pow(z, j) * math.gamma(j + 0.5)) / math.factorial(j);
                sum += nextTerm;
                nextTerm *= (z * (j + 0.5) / (j + 1.0));
            }
            return 1.0 - math.sqrt(1.0 - z) * (1.0 + sum / math.sqrt(math.PI));
        } else {
            let k = (n - 1) / 2;
            let sum = 0.0;
            // Note: math.gamma(1.5) == math.sqrt(math.PI) / 2.0.
            let nextTerm = 2.0 * z / math.sqrt(math.PI);
            for (let j = 1; j <= k; ++j) {
                //sum += (math.pow(z, j) * math.factorial(j - 1)) / math.gamma(j + 0.5);
                sum += nextTerm;
                nextTerm *= (z * j) / (j + 0.5);
            }
            return arcsineCdf(z) - math.sqrt(1.0 - z) * sum / math.sqrt(z * math.PI);
        }
    };

    // Standard normal distribution.
    /*public*/ ropufu.distNormal = function () {
        const SQRT1_2PI = math.SQRT1_2 / math.sqrt(math.PI);
        return {
            support: [Number.NEGATIVE_INFINITY, Number.POSITIVE_INFINITY],
            cdf: function (x) {
                return (1.0 + math.erf(x / math.SQRT2)) / 2.0;
            },
            pdf: function (x) {
                return SQRT1_2PI * math.exp(-x * x / 2.0);
            },
            quantile: function (p, absoluteTolerance = ropufu.epsilon) {
                if (p == 0)
                    return this.support[0];
                if (p == 1)
                    return this.support[1];
                return numericalQuantile(p, (x) => this.cdf(x), (x) => this.pdf(x), absoluteTolerance);
            }
        };
    };

    // Student's t distribution.
    /*public*/ ropufu.distStudent = function (n) {
        if (!math.isInteger(n))
            return bad;
        if (n <= 0)
            return bad;

        // Pre-compute normalizing constant.
        let c = math.gamma((n + 1) / 2.0) / (math.sqrt(n * math.PI) * math.gamma(n / 2.0));
        return {
            support: [Number.NEGATIVE_INFINITY, Number.POSITIVE_INFINITY],
            df: n,
            cdf: function (x) {
                if (math.isPositive(x))
                    return 1.0 - 0.5 * arithmeticBetaCdf(this.df / (x * x + this.df), this.df);
                else
                    return 0.5 * arithmeticBetaCdf(this.df / (x * x + this.df), this.df);
            },
            pdf: function (x) {
                return c * math.pow((x * x + this.df) / this.df, -(this.df + 1) / 2.0);
            },
            quantile: function (p, absoluteTolerance = ropufu.epsilon) {
                if (p == 0)
                    return this.support[0];
                if (p == 1)
                    return this.support[1];
                return numericalQuantile(p, (x) => this.cdf(x), (x) => this.pdf(x), absoluteTolerance);
            }
        };
    };

    // Empirical quantile from data.
    /*public*/ ropufu.orderStatistic = function (data) {
        // Parse data if it is a string.
        if (typeof data === "string" || data instanceof String) {
            const words = data.split(",");
            const parsed = Array(words.length);
            for (let i = 0; i < words.length; ++i) {
                if (words[i].length == 0)
                    return bad;
                parsed[i] = Number(words[i]);
            }
            data = parsed;
        }

        // Make sure we have a non-empty array.
        if (!Array.isArray(data) || data.length == 0)
            return bad;

        // Validate array.
        let n = data.length;
        let sum = 0;
        let sumOfSquares = 0;
        for (let i = 0; i < n; ++i) {
            let x = data[i];
            if (!Number.isFinite(x))
                return bad;
            sum += x;
            sumOfSquares += (x * x);
        }
        let average = sum / n;
        let sumOfSquaredDistances = sumOfSquares - (sum * sum) / n;

        data.sort(function (a, b) { return a - b; });

        return {
            sorted: data,
            mean: average,
            sdPopulation: Math.sqrt(sumOfSquaredDistances / n),
            sdSample: Math.sqrt(sumOfSquaredDistances / (n - 1)),
            whichQuantile: function (value, isStrict = false) {
                value = parseNumber(value);
                if (Number.isNaN(value))
                    return Number.NaN;

                if (isStrict) {
                    for (let i = 0; i < n; ++i)
                        if (this.sorted[i] >= value)
                            return i / n;
                    return 1.0;
                } else {
                    for (let i = 0; i < n; ++i)
                        if (this.sorted[i] > value)
                            return i / n;
                    return 1.0;
                }
            }
        };
    };
} (window.ropufu = window.ropufu || {}, jQuery));
