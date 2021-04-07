

// Workaround: File input value reset
function resetFileInput(input) {
    $(input).replaceWith($(input).val('').clone(true));
}

/**
 * Makes an object able to emit events
 */
window.makeEventEmitter = function (object) {
    $.extend(object, {
        _events: {},

        addEventListener: function (name, handler) {
            if (this._events.hasOwnProperty(name)) {
                this._events[name].push(handler);
            }
            else {
                this._events[name] = [handler];
            }
        },

        removeEventListener: function (name, handler) {
            /* This is a bit tricky, because how would you identify functions?
               This simple solution should work if you pass THE SAME handler. */
            if (!this._events.hasOwnProperty(name)) {
                return;
            }

            var index = this._events[name].indexOf(handler);
            if (index !== -1) {
                this._events[name].splice(index, 1);
            }
        },

        fireEvent: function (name, args) {
            if (!this._events.hasOwnProperty(name)) {
                return;
            }

            if (!args || !args.length) {
                args = [];
            }

            var evs = this._events[name], l = evs.length;
            for (var i = 0; i < l; i++) {
                evs[i].apply(null, args);
            }
        }
    });
};



/*
Prevents autofilling of autocomplete= "off" fields.
Field must initially be readonly first for this to work.
*/

var clearAutofocus = function () {
    $('[readonly][autocomplete="off"]').removeAttr('readonly');

    $(document)
        .on('focus',
            '[autocomplete="off"]',
            function () {
                this.removeAttribute('readonly');
            });

    var $autofocus = $('[autofocus]');
    if ($autofocus && $autofocus[0]) {
        $autofocus[0].focus();
    }
};

$(clearAutofocus);


var closeParentVexModal = function (el) {
    var parentEl = $(el).closest('.vex');
    if (!parentEl) {
        return;
    }
    var parentModal = Object.values(vex.getAll())
        .filter(function (x) { return x.rootEl === parentEl[0]; })[0];

    var promise = new $.Deferred();

    parentModal.options.afterClose = promise.resolve;
    if (parentModal) {
        parentModal.close();
    }
    return promise;
};

(function ($) {
    // Add the client timezone to the cookie to be retrieved in server side
    document.cookie = 'timezoneoffset=' + new Date().getTimezoneOffset();

    // Used to serialize a form as a javascript object.
    $.fn.serializeObject = function () {
        var result = {};
        var extend = function (i, element) {
            var node = result[element.name];

            /*
            If node with same name exists, need to convert it to an array
            as it is a multi-value field (i.e., checkboxes)
            */

            if ('undefined' !== typeof node && node !== null) {
                if ($.isArray(node)) {
                    node.push(element.value);
                } else {
                    result[element.name] = [node, element.value];
                }
            } else {
                result[element.name] = element.value;
            }
        };

        $.each(this.serializeArray(), extend);

        var keys = Object.keys(result);
        for (var j = 0; j < keys.length; j++) {
            var key = keys[j];
            var array;

            if (!key.includes('[]')) {
                continue;
            }

            if ($.isArray(result[key])) {
                array = result[key];
            }
            else {
                array = [result[key]];
            }

            for (var k = 0; k < array.length; k++) {
                var newKey = key.replace('[]', '[' + k + ']');
                result[newKey] = array[k];
            }
            delete result[key];
        }

        var formData = this.serializeFiles();

        $.each(result, function (i, val) {
            formData.append(i, val);
        });

        return formData;
    };

    // Used to serialize all file type inputs to object with the file data
    $.fn.serializeFiles = function () {
        var $obj = $(this);
        var formData = new FormData();
        $.each($obj.find('input[type="file"]'), function (i, tag) {
            $.each($(tag)[0].files, function (index, file) {
                formData.append(tag.name, file);
            });
        });
        return formData;
    };


    $.ajaxSetup({
        cache: false,
        async: true,
        beforeSend: function (xhr) {
            xhr.setRequestHeader('pageid', window.name);
        }
    });
})(jQuery);

var base = (function () {
    var ERKOSMS = window.ERKOSMS = window.ERKOSMS || {};

    // #region Blocker
    var blocker = null;
    var blockCount = 0;

    var blockerActiveTimeout = 500;
    var blockerSpinTimeout = 2000;

    var blockInput = function () {
        blockCount++;
        if (blocker) {
            return blocker;
        }
        blocker = $(document.createElement('div'));
        blocker.addClass('blocker');
        blocker.addClass('passive');

        var $blockerContainer = $(document.createElement('div'));
        $blockerContainer.addClass('blockerContainer');
        blocker.append($blockerContainer);

        var $spin = $(document.createElement('div'));
        $spin.data('role', 'preloader');
        $spin.data('type', 'ring');
        $spin.addClass('mif-spinner1');
        $spin.addClass('mif-ani-pulse');
        $spin.addClass('mif-ani-fast');
        $spin.addClass('blocker-spin');
        $spin.addClass('passive');

        $(document.body).append(blocker);
        $blockerContainer.append($spin);

        var $blockerText = $(document.createElement('div'));
        $blockerText.addClass('blockerText');
        $blockerText.text(window.ERKOSMS.BlockerLoadingText);
        $blockerContainer.append($blockerText);

        blocker.fadeIn(blockerActiveTimeout);
        $($spin).fadeIn(blockerSpinTimeout);

        return blocker;
    };

    var unblockInput = function () {
        blockCount--;
        if (blocker && blockCount === 0) {
            blocker.remove();
            blocker = null;
        }
    };
    // #endregion

    window.makeEventEmitter(ERKOSMS);

    ERKOSMS.formatUtc = function (date) {
        return window.moment(date).format( ERKOSMS.CustomDateFormat + ' hh:mm:ss.SSS A');
    };

    ERKOSMS.formatUtcShort = function (date) {
        return window.moment(date).format(ERKOSMS.CustomDateFormat+' hh:mm:ss A');
    };

    ERKOSMS.formatDuration = function (duration) {
        return Math.floor(duration.asHours()) + ':' +
            duration.minutes().toString().padStart(2, '0') + ':' +
            duration.seconds().toString().padStart(2, '0');
    };


    ERKOSMS.ShowGeneralError = function () {
        vex.dialog.alert(ERKOSMS.SystemErrorMessage);
    };

    var getAjaxData = function (element) {
        if (!element) {
            return {};
        }
        var dt = element.data('erkosms-ajax-data');
        if (typeof dt === 'function') {
            return dt() || {};
        }
        return dt || {};
    };


    /* Defines the possible return codes from an AJAX call
     * Mirrors AjaxResultCode enum in the back-end code.
     * Any changes made to this enum must be reflected to the back-end.
     */
    ERKOSMS.ResultCode = {
        None: 0,
        Success: 1,
        UserFailure: 2,
        NotLoggedIn: 3
    };

    ERKOSMS.AjaxCall = function (action, settings, disableBlock) {
        if (ERKOSMS.connected === false) {
            if (!disableBlock) {
                ERKOSMS.ShowConnectionError();
            }
            return (new $.Deferred).reject(null, true);
        }

        if (settings && settings.async === false) {
            disableBlock = true;
        }
        if (!disableBlock) {
            blockInput();
        }

        var xhr = $.ajax(action, $.extend({}, settings));
        xhr.always(function () {
            if (!disableBlock) {
                unblockInput();
            }
        });

        return xhr;
    };

    ERKOSMS.GetAjaxView = function (action, settings, disableBlock) {
        return ERKOSMS.AjaxAction(action, settings, disableBlock)
            .then(function (result) {
                return $(result);
            });
    };

    ERKOSMS.GetAjaxDialog = function (action, settings, disableBlock) {
        return ERKOSMS.GetAjaxView(action, settings, disableBlock)
            .then(function (result) {
                var dialog = vex.open({
                    buttons: [],
                    showCloseButton: true,
                    focusFirstInput: false,
                    escapeButtonCloses: true,
                    overlayClosesOnClick: false,
                    contentClassName: 'erkosms-vex-dialog'
                });
                var $dialogContent = $(dialog.contentEl);
                $dialogContent.append(result);

                clearAutofocus();
                var autofocus = result.find('[autofocus]');
                if (autofocus && autofocus[0]) {
                    autofocus[0].focus();
                }
                return dialog;
            },
                function (data, handled) {
                    if (handled !== true) {
                        ERKOSMS.ShowGeneralError();
                    }
                    return (new $.Deferred).reject(data, true);
                });
    };

    ERKOSMS.AjaxAction = function (action, settings, disableBlock) {
        return ERKOSMS.AjaxCall(action, settings, disableBlock)
            .then(function (result) {
                if (result.Code === ERKOSMS.ResultCode.UserFailure) {
                    ERKOSMS.ShowWarning(result.Message);
                    return (new $.Deferred).reject(result, true);
                }
                else if (result.Code === ERKOSMS.ResultCode.NotLoggedIn) {
                    window.location = '/Login';
                    return (new $.Deferred).reject(result, true);
                }
                return result;
            }, function (result, handled) {
                return (new $.Deferred).reject(result, handled || false);
            });
    };

    ERKOSMS.CreateGUID = function () {
        var s4 = function () {
            return (
                Math.floor(
                    Math.random() * 0x10000 /* 65536 */
                ).toString(16)
            );
        };
        return (
            s4() + s4() + '-' +
            s4() + '-' +
            s4() + '-' +
            s4() + '-' +
            s4() + s4() + s4()
        );
    };

    ERKOSMS.AjaxSubmit = function (form, settings, disableBlock) {
        var data = form.serializeObject();

        var formData = {
            data: data,
            method: form.prop('method'),
            cache: false,
            processData: false,
            contentType: false
        };

        var action = form.prop('action');

        if (!action) {
            var button = form.find('button[type=submit][formaction]:not(:hidden)');
            if (button) {
                action = button.attr('formaction');
            }
        }

        return ERKOSMS.AjaxAction(action, $.extend(settings, formData), disableBlock);
    };

    ERKOSMS.topologyTreeNodeModes = {
        HideCheckbox: 0,
        Normal: 1
    };

    // topology tree's checkboxes modes being set by that object.
    ERKOSMS.TopologyTreeMode = {
        selectionMode: ERKOSMS.topologyTreeNodeModes.Normal,

        getSelectionMode: function () {
            return this.selectionMode;
        },

        setSelectionMode: function (topologyTreeNodeModes) {
            this.selectionMode = topologyTreeNodeModes;
            ERKOSMS.TopologyTree.setCheckboxes(this.selectionMode);
        }
    };

    ERKOSMS.IsBrowserIE = function () {
        return window.navigator.userAgent.match(/Trident/);
    };

    ERKOSMS.getMaximumCanvasSize = function () {
        if (ERKOSMS.IsBrowserIE()) {
            return 8192;
        }
        return 32767;
    };

    ERKOSMS.showComplexValidationMessage = function (validationMessage, element, error) {

        var errorDivName = element.violatedRule + "ErrorDiv";
        var errorItemListDivName = element.violatedRule + "ItemList";

        //Check if general message applied to page
        var generalMessageApplied = $('#formErrors label').filter(function () {
            return $(this)[0].innerHTML === validationMessage;
        }).length;

        //General message already applied
        if (generalMessageApplied > 0) {

            ////new display name error came
            var itemExist = false;

            $.each($('#' + errorItemListDivName + ' label'),
                function (index, value) {
                    if ($(value).text() === $(error[0]).text().trim()) {
                        itemExist = true;
                    }
                });

            if (itemExist === false) {
                var liElement = $('<li style="color:red"></li>').append($(error[0]).css('cursor', 'auto'));
                $("#" + errorItemListDivName).append(liElement);
            }

        } //Same tagname error occurring first time
        else {

            var errorLabel = '<label class="error validation" style="cursor:auto">' +
                validationMessage +
                '</label>';
            var sameDisplayNameErrorContainer = '<div id="' +
                errorDivName +
                '">' +
                errorLabel +
                '<ul id="' +
                errorItemListDivName +
                '"></ul></div>';
            $(sameDisplayNameErrorContainer).prependTo("#formErrors");
            var liElement = $('<li style="color:red"></li>').append($(error[0]).css('cursor', 'auto'));
            $("#" + errorItemListDivName).append(liElement);
            return;
        }

    }

    //sets guid for pertab.
    if (!window.name) {
        window.name = ERKOSMS.CreateGUID();
    }

    $(document).on('click', '[data-erkosms-role="ajax-dialog-button"]',
        function (ev) {
            ev.preventDefault();
            var $tg = $(ev.currentTarget);
            var action = $tg.data('erkosms-targetaction');
            var elementAjaxData = getAjaxData($tg);

            ERKOSMS.GetAjaxDialog(action, { data: elementAjaxData })
                .then(function (result) {
                    var data = { result: result };
                    $tg.trigger('ajax-dialog:succeed', data);
                    return result;
                }, function (message) {
                    var data = { message: message };
                    $tg.trigger('ajax-dialog:failed', data);
                    return message;
                });
        });

    var confirmFormCallback = function (form, obj, result) {
        if (result) {
            ERKOSMS.AjaxSubmit(form).then(function (result) {
                var data = { data: obj, result: result };
                form.trigger('ajax-form:succeed', data);
            }, function (message) {
                var data = { data: obj, message: message };
                form.trigger('ajax-form:failed', data);
            });
        } else {
            form.trigger('ajax-action:cancelled');
        }
    };

    $(document).on('submit', '[data-erkosms-role="ajax-form"]', function (ev) {
        ev.preventDefault();
        var $tg = $(ev.currentTarget);
        var $form = $tg;
        var obj = $form.serializeObject();

        var autoclose = $form.data('ajax-form-autoclose');

        $tg.on('ajax-form:' + autoclose, function () {
            closeParentVexModal($form);
        });

        if ($form.data("ajax-form-confirmation") === "confirm") {
            vex.dialog.confirm({
                message: $tg.data('confirmation-message') || '',
                callback: confirmFormCallback.bind(null, $form, obj)
            });
        } else {
            ERKOSMS.AjaxSubmit($form).then(function (result) {
                var data = { data: obj, result: result };
                $tg.trigger('ajax-form:succeed', data);
            }, function (message) {
                var data = { data: obj, message: message };
                $tg.trigger('ajax-form:failed', data);
            });
        }
    });

    // When openning with new page with form apppend the page id
    //to the form before submit.
    $(document).on('submit',
        'form[target="_blank"]:not([data-erkosms-role="ajax-form"])',
        function (ev) {
            ev.preventDefault();
            var $tg = $(ev.currentTarget);
            var $form = $tg;
            if ($form.find('input#pageid').length === 0) {
                $('<input>').attr({
                    type: 'hidden',
                    id: 'pageid',
                    name: 'pageid',
                    value: window.name
                }).appendTo($form);
            }
            this.submit();
            return false;
        });

    var confirmActionCallback = function (tg, result) {
        if (result) {
            var action = tg.data('erkosms-targetaction');
            tg.trigger('ajax-action:confirmed');
            var elementAjaxData = getAjaxData(tg);
            ERKOSMS.AjaxAction(action, { method: 'POST', data: elementAjaxData })
                .then(function (data) {
                    tg.trigger('ajax-action:succeed', [data]);
                },
                    function (failData) {
                        tg.trigger('ajax-action:failed', [failData]);
                    });
        } else {
            tg.trigger('ajax-action:cancelled');
        }
    };

    $(document).on('click', '[data-erkosms-role="ajax-confirmation"]',
        function (ev) {
            ev.preventDefault();
            var $tg = $(ev.currentTarget);

            vex.dialog.confirm({
                message: $tg.data('confirmation-message') || '',
                callback: confirmActionCallback.bind(null, $tg)
            });
        });

    $(document).on('click', '[data-erkosms-role="ajax-action-button"]',
        function (ev) {
            ev.preventDefault();
            var $tg = $(ev.currentTarget);
            confirmActionCallback($tg, true);
        });


    $(document)
        .on('change',
            '[data-erkosms-role="ajax-upload"] input[type="file"]',
            function () {
                if (document.activeElement && document.activeElement !== document.body) {
                    document.activeElement.blur();
                }
                if (!this.value) {
                    return;
                }
                var parent = $(this).closest('[data-erkosms-role="ajax-upload"]');

                var targetSelector = parent.data('upload-target');
                var target = parent;
                if (targetSelector) {
                    target = $(targetSelector);
                }
                var image = target.find('img').first();
                var hidden = target.find('> input[type="hidden"]');
                var $fileInput = $(this);

                var action = target.data('erkosms-targetaction')
                    || 'MediaResource/CreateMediaResource';


                var file = this.files[0];
                var fileName = file.name.toLowerCase();
                var fileTypeValidationMessage = $.validator.messages.fileType;
                var regEx = new RegExp("([a-zA-Z0-9\s_\\.\-:])+(.png|.jpg|.jpeg|.gif|.tiff|.tif|.pjp|.svg|.svgz|.bmp|.webp|.ico|.xbm|.dib|.pjpeg|.avif)$");

                if (!regEx.exec(fileName)) {
                    vex.dialog.alert(fileTypeValidationMessage);
                    resetFileInput($fileInput);
                    return;
                }

                var maxSize = target.data('max-size');
                var maxSizeMessage = target.data('max-size-message');

                if (maxSize && file.size > maxSize) {
                    vex.dialog.alert(maxSizeMessage);
                    resetFileInput($fileInput);
                    return;
                }

                var data = new FormData();
                data.append('file', file);

                var ajax = ERKOSMS.AjaxAction(action, {
                    data: data,
                    processData: false,
                    cache: false,
                    contentType: false,
                    method: 'POST'
                });

                var failCallback = function () {
                    resetFileInput($fileInput);
                };

                ajax.done(function (result) {
                    image.attr('src', result.Data.relativePath);
                    parent.removeClass('notSelected');
                    hidden.val(result.Data.fileId);
                });

                ajax.fail(failCallback);
            });

    $(document)
        .on('click',
            '[data-erkosms-role="ajax-upload"] .removeButton',
            function () {
                var parent = $(this).closest('[data-erkosms-role="ajax-upload"]');
                var image = parent.find('img');
                var hidden = parent.find('input[type="hidden"]');
                var fileInput = parent.find('input[type="file"]');

                image.attr('src', '');
                parent.addClass('notSelected');
                hidden.val('');

                resetFileInput(fileInput);
            });

    $(document).on('dblclick', 'input[type="file"],' +
        ' [data-erkosms-role="ajax-dialog-button"], ' +
        ' [data-erkosms-role="ajax-confirmation"],' +
        ' [data-erkosms-role="ajax-action-button"]',
        function (ev) {
            ev.preventDefault();
            ev.stopPropagation();
            return false;
        });

    var baseWarningDialog;
    ERKOSMS.ShowWarning = function (message) {
        message = message || '';
        if (baseWarningDialog) {
            var newMessageDiv = document.createElement('div');
            newMessageDiv.className = 'vex-dialog-message';

            newMessageDiv.innerHTML = message.unsafeMessage || message;
            var vexMessages = baseWarningDialog.form.querySelectorAll('.vex-dialog-message');
            baseWarningDialog.form.insertBefore(newMessageDiv, vexMessages[vexMessages.length - 1].nextSibling);
        }
        else {
            baseWarningDialog = vex.dialog.alert({ unsafeMessage: message });
            baseWarningDialog.options.beforeClose = function () {
                baseWarningDialog = null;
            };
        }
    };

    return {
        showLoadingGif: function () {
            blockInput();
        },
        hideLoadingGif: function () {
            unblockInput();
        }
    }
})();



/*
Implicit label association Labels look for 'id' with 'for' attributes first.
Then looks for 'name' in the same form with 'for' attribute.
Then looks at the first sibling input element.
*/

$(function () {
    $('body')
        .on('click',
            'label',
            function (e) {
                var $this = $(this);
                var labelFor = $this.attr('for');
                var input = document.getElementById(labelFor);

                if (!input) {
                    input = $this.find('input')[0];
                }

                if (input) {
                    return;
                }

                input = $this.closest('form')
                    .find('[name="' + labelFor + '"]')[0];

                if (!input) {
                    input = $(this).siblings('.' + labelFor)[0];
                }

                if (!input) {
                    input = $(this).siblings('input')[0];
                }

                if (!input) {
                    input = $(this).siblings().find('input')[0];
                }

                if (input) {
                    input.focus();
                    e.preventDefault();
                    e.stopPropagation();
                    return false;
                }
            });
});
