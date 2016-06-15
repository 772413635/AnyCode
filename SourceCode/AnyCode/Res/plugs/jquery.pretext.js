/*!
 * jQuery Pretext - Add pretext labels to input fields.
 *     https://github.com/stonebk/jquery-pretext
 */
(function ($) {

    /**
     * Add pretext to the given input using the given label.
     * @param {object} $input The jQuery input element.
     * @param {object} $label The jQuery label element.
     * @constructor
     */
    $.Pretext = function ($input, $label) {

        /**
         * The jQuery input element.
         * @type object
         */
        this.$input = $input;

        /**
         * The jQuery label element.
         * @type object
         */
        this.$label = $label;

        /**
         * The jQuery element housing the input element.
         * @type object
         */
        this.$container = $(document.createElement('div'));

        // Initialize the pretext object.
        this.init();
    };

    /**
     * The class name identifying pretext elements.
     * @type String
     * @static
     */
    $.Pretext.CLASSNAME = 'pretext';

    /**
     * The default style for the prefix elements.
     * @type String
     * @static
     */
    $.Pretext.STYLE = [
        // Container CSS
        '.', $.Pretext.CLASSNAME, '-container {',
        'position: relative;',
        'display: inline-block;',
        '}',

        // Label CSS
        '.', $.Pretext.CLASSNAME, '{',
        'position: absolute;',
        'top: 0;',
        'left: 0;',
        'cursor: text;',
        'color: #a9a9a9;',
        'font-size: 13.333px;',
        'font-weight:normal;',
        'text-align:left',
        '}'
    ].join('');

    /**
     * Static init for the pretext object. Add CSS to the DOM.
     * @static
     */
    $.Pretext.init = function () {
        $(document.createElement('style'))
            .text($.Pretext.STYLE)
            .prependTo('head');
        $.Pretext.initialized = true;
    };

    // Extend the prototype
    $.Pretext.prototype = {

        /**
         * Initialize the pretext object.
         */
        init: function () {

            // Listen for events that alter the value of the input
            this.$input.bind('change keydown', $.proxy(this.onChange, this));

            // Initialize the pretext container
            this.$container
                .addClass($.Pretext.CLASSNAME + '-container')
                .insertAfter(this.$input)
                .append(this.$input)
                .append(this.$label);

            // Initialize the pretext label
            this.$label.addClass($.Pretext.CLASSNAME);

            // Initialize the pretext state
            this.onChange();
        },

        /**
         * Handle events that change the value of the input.
         */
        onChange: function () {
            // Add a short delay to allow value to change after keydown
            setTimeout($.proxy(function () {
                if (this.$input.val() === '') {
                    this.$label.show();
                } else {
                    this.$label.hide();
                }
            }, this), 1);
        }
    };

    // Add "pretext" as a jquery plugin
    $.fn.pretext = function (options) {
        // Initialize the object on first use
        if (!$.Pretext.initialized) {
            $.Pretext.init();
        }
        return this.each(function () {
            var $this = $(this),
                $label = $('label[for="' + $this.attr('id') + '"]');

            // Make sure a label exists and is not already initialized
            if ($label.length > 0 && !$label.hasClass($.Pretext.CLASSNAME)) {
                (new $.Pretext($this, $label));
            }
        });
    };

}(jQuery));
