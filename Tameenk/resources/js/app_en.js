/* || ===

= IMKAN.com.sa
= APP FIRE

=== || */


$(document).ready(function () {

    // Featured Slider ..
    $('#top-slider, .top-slider').slick({
        dots: true,
        rtl: true,
        slidesToShow: 1,
        slidesToScroll: 1,
        arrows: false,
        autoplay: true,
        autoplaySpeed: 5000,
        asNavFor: '.slides .slider',
        appendDots: $(".arrows-move")
    });
    $('.slides .slider').slick({
        dots: false,
        rtl: true,
        asNavFor: '#top-slider, .top-slider',
        arrows: false,
        slidesToShow: 1,
        slidesToScroll: 1,
        autoplay: true,
        autoplaySpeed: 5000
    });

    // Partners
    $('.img-list').slick({
        dots: false,
        rtl: true,
        slidesToShow: 7,
        slidesToScroll: 1,
        arrows: true,
        autoplay: true,
        autoplaySpeed: 5000,
        nextArrow: document.getElementById('prev-comp'),
        prevArrow: document.getElementById('next-comp'),
        appendDots: $(".arrows-move"),
        responsive: [{
            breakpoint: 1024,
            settings: {
                slidesToShow: 4,
                slidesToScroll: 4,
                infinite: true,
                dots: true
            }
        },
        {
            breakpoint: 600,
            settings: {
                slidesToShow: 2,
                slidesToScroll: 2
            }
        },
        {
            breakpoint: 480,
            settings: {
                slidesToShow: 4,
                slidesToScroll: 4
            }
        }

        ]
    });

    // Top
    $('.menuToggle').click(function () {
        $('.mobile-nav ').toggleClass("active");
    });
    $('.site-nav .menu .navTrigger').click(function () {
        $('.site-nav .dropdown').toggleClass("active");
        $(this).toggleClass("active");
    });

    // Tabs ..
    $('.payments-cards article header').click(function () {
        $('.payments-cards article').removeClass("active");
        $(this).parent().addClass("active");
    });

    // Login .. 
    $('#login-tabs .current-active, .login-tabs .current-active').on('click', function () {
        $('#login-tabs, .login-tabs').toggleClass('active-register');
    });

    // tabs
    $('#carsLink, .carsLink').click(function () {
        $('.tabs-section .tabs a').parent().addClass("not-now");
        $('.tab-content').removeClass("active");
        $('.tab-content#cars, .tab-content.cars').addClass("active");
        $(this).parent().removeClass("not-now");
    });
    $('#travelLink, .travelLink').click(function () {
        $('.tabs-section .tabs a').parent().addClass("not-now");
        $('.tab-content').removeClass("active");
        $('.tab-content#travel, .tab-content.travel').addClass("active");
        $(this).parent().removeClass("not-now");
    });
    $('#homeLink, .homeLink').click(function () {
        $('.tabs-section .tabs a').parent().addClass("not-now");
        $('.tab-content').removeClass("active");
        $('.tab-content#home, .tab-content.home').addClass("active");
        $(this).parent().removeClass("not-now");
    });
    $('#medicalLink, .medicalLink').click(function () {
        $('.tabs-section .tabs a').parent().addClass("not-now");
        $('.tab-content').removeClass("active");
        $('.tab-content#medical, .tab-content.medical').addClass("active");
        $(this).parent().removeClass("not-now");
    });

    // tabs profile
    $('aside ul li a').click(function () {
        $('.content-view').removeClass("active");
        $('aside ul li').removeClass('active');
        $($(this).data('toshow')).addClass("active");
        $(this).parent().addClass("active");
    });
    $('#profileEditURL, .profileEditURL').click(function () {
        $('.content.content-view').removeClass("active");
        $('.content#profileEdit, .content.profileEdit').addClass("active");
    });
    $('#addAddressURL, .addAddressURL').click(function () {
        $('.content.content-view').removeClass("active");
        $('.content#addAddress, .content.addAddress').addClass("active");
    });
    $('#addBankURL, .addBankURL').click(function () {
        $('.content.content-view').removeClass("active");
        $('.content#addBank, .content.addBank').addClass("active");
    });
    $('#editsearch, .editsearch').click(function () {
        $('.search-info').removeClass("active");
        $('#edit-search-info, .edit-search-info').addClass("active");
    });
    $('#showsearch, .showsearch').click(function () {
        $('.search-info').removeClass("active");
        $('#show-search-info, .show-search-info').addClass("active");
    });
    $('#editDataLink, .editDataLink').click(function () {
        $('.search-result-info').removeClass("active");
        $('#edit-data, .edit-data').addClass("active");
    });
    $('#saveDataLink, .saveDataLink').click(function () {
        $('.search-result-info').removeClass("active");
        $('#show_data, .show_data').addClass("active");
    });

    // Sorting Based ..
    $('#show-range-prices, .show-range-prices').click(function () {
        $('.price-select').toggleClass("active");
    });

    // Show Extra Features ..
    $('.show-extra-features').click(function () {
        $($(this).data('toshow')).toggleClass("active");
        return false;
    });

    // Add new Driver ..
    $('#add-driver, .add-driver').click(function () {
        var row = $('#driverspace .row, .driverspace .row');
        if ($('#driverspace .remove:visible, .driverspace .remove:visible').length == 0) {
            row.show();
        } else {
            $(row[0]).find('select.select2').select2('destroy');

            var clone = $(row[0]).clone();
            $(clone).find('[for]').each(function (i) {
                var target = $(this).attr('for');
                $(this).attr('for', `${target}_${i}`).parent().find(`[id="${target}"]`).attr('id', `${target}_${i}`);
            });
            clone.appendTo($('#driverspace, .driverspace'));

            //$(clone).find('.selectinside').removeClass('clicked');
            $(clone).find('select.select2').select2();
            $(row[0]).find('select.select2').select2();
        }
        return false;
    });

    // Remove new Driver
    $('body').on('click', '#driverspace .remove, .driverspace .remove', function () {
        var row = $(this).closest('.row');
        if ($('#driverspace .remove, .driverspace .remove').length == 1) {
            row.hide();
        } else {
            row.remove();
        }
        return false;
    });

    // Filters Show & Hide ..
    $('.toggle-filters').click(function () {
        if ($('#general-view').hasClass('active')) {
            $('#general-view').removeClass("active");
            $('#price-view').removeClass("active");
        } else {
            $('#general-view').addClass("active");
            $('#price-view').addClass("active");
        }
        $('.full-view').toggleClass('active');
    });

    // Filters Select ..
    $('.select-list li').click(function () {
        $($(this).data('tohide')).removeClass('active');
        $(this).toggleClass("active");
    });

    // Single Features ..
    $('.single-feature:not(.disabled)').click(function () {
        $(this).toggleClass("active");
    });

    // View More ..
    $('article .openMore').click(function () {
        $(this).closest('article').toggleClass("active");
        $(this).html($(this).text() == 'Hide Details' ? 'View Detailed' : 'Hide Details');
    });

    // Add To Compare ..
    $('.cd-add-to-cart').click(function () {
        $(this).parent().toggleClass("active");
        $(this).html($(this).text() == '<i class="ic-plus"></i> Delete Comparison' ? ' <i class="ic-plus"></i> Add Comparison' : '<i class="ic-plus"></i> Delete Comparison');
    });

    // Show & Hide Password ..
    $('.show-password').hover(function () {
        $('.pass-to-show').attr('type', 'text');
    }, function () {
        $('.pass-to-show').attr('type', 'password');
    });

    // Registred Car ..
    $('.registerCheckClass').change(function () {
        $('.registerdCarInput').toggleClass("active");
        $('.divOwner').toggleClass("switchin");
        if ($('.divOwner').hasClass("switchin")) {
            if ($('#IsOwner').val() == "false") {
                $("input[name='OwnerId']").val("");
                $(".owenerCheck").click();
            }
        }
        $('.registerdCarLabel').text($(this).hasClass('customCard') ? 'Registration Number' : 'Custom Card Number');
    });

    // Add Driver .. 
    $(".addnewdriverCheck").change(function () {
        $(".driver-fields").toggleClass("hidden");
    });

    // Owner Check ..
    $('.owenerCheck').change(function () {
        $(".ownerIdInput").toggleClass("active");
    });

    // Select Update ..
    $('.select2').select2();
    $('body').on('click', '.selectinside', function () {
        $(this).find('option.removeonfocus').remove();
        $(this).addClass('clicked');
    });

    // Edit Page ..
    $(function () {
        $('#selectres, .selectres').change(function () {
            $('.options-list').addClass('hidden');
            $('#' + $(this).val()).removeClass('hidden');
        });
    });

    // Close Modal ..
    $('.closeThis').click(function () {
        $('#CompareModal, .CompareModal').removeClass('active');
    });


    var gregorianDate = {
        currentYear: (new Date()).getFullYear(),
        months: ['January', 'February', 'March', 'April', 'May', 'June', 'July', 'August', 'September', 'October', 'November', 'December'],
        weekdays: ['Sunday', 'Monday', 'Tuesday', 'Wednesday', 'Thursday', 'Friday', 'Saturday'],
        weekdaysShort: ['Sun', 'Mon', 'Tue', 'Wed', 'Thu', 'Fri', 'Sat']
    };
    var hijriDate = {
        currentYear: Math.round((gregorianDate.currentYear - 622) * (33 / 32)),
        months: ['محرم', 'صفر', 'ربيع الأول', 'ربيع الثاني', 'جمادي الأول', 'جمادي الثاني', 'رجب', 'شعبان', 'رمضان', 'شوال', 'ذو القعدة', 'ذو الحجة'],
        weekdays: ['الأحد', 'الإثنين', 'الثلاثاء', 'الأربعاء', 'الخميس', 'الجمعة', 'السبت'],
        weekdaysShort: ['الأحد', 'الإثنين', 'الثلاثاء', 'الأربعاء', 'الخميس', 'الجمعة', 'السبت']
    };
    window.hijriNumber = function (number) {
        var keymap = ['۰', '۱', '۲', '۳', '۴', '۵', '۶', '۷', '۸', '۹'];
        return (number + '').replace(/[0-9]/g, function (index) {
            return keymap[+index]
        });
    }

    var add_days = function (date, days) {
        var result = new Date(date);
        result.setDate(result.getDate() + days);
        return result;
    }

    // Car Registration ..
    var from = new Pikaday({
        field: $('#endcarregister, .endcarregister')[0],
        firstDay: 1,
        minDate: add_days(new Date, 1),
        maxDate: add_days(new Date, 14),
        format: 'D/M/YYYY',
        toString(date, format) {
            // you should do formatting based on the passed format,
            // but we will just return 'D/M/YYYY' for simplicity
            const day = date.getDate();
            const month = date.getMonth() + 1;
            const year = date.getFullYear();
            return `${day}/${month}/${year}`;
        },
        parse(dateString, format) {
            // dateString is the result of `toString` method
            const parts = dateString.split('/');
            const day = parseInt(parts[0], 10);
            const month = parseInt(parts[1] - 1, 10);
            const year = parseInt(parts[1], 10);
            return new Date(year, month, day);
        },
        i18n: gregorianDate
    });
    from.setDate(new Date());

    // Car Registration ..
    var from = new Pikaday({
        field: $('#hijrisample, .hijrisample')[0],
        firstDay: 1,
        minDate: new Date(1420, 1, 1),
        maxDate: new Date(1441, 12, 31),
        yearRange: [1420, 1441],
        toString(date, format) {
            //return hijriNumber(date.getDate()) + '/' + hijriDate.months[date.getMonth()] + '/' + hijriNumber(date.getFullYear())
            var day = date.getDate();
            var month = date.getMonth() + 1;
            var year = date.getFullYear();
            if ((day + '').length == 1) day = '0' + day;
            if ((month + '').length == 1) month = '0' + month;
            return `${day}/${month}/${year}`;
        },
        parse(dateString, format) {
            // dateString is the result of `toString` method
            const parts = dateString.split('/');
            const day = parseInt(parts[0], 10);
            const month = parseInt(parts[1] - 1, 10);
            const year = parseInt(parts[1], 10);
            return new Date(year, month, day);
        },
        // Hijri Converting ..
        i18n: gregorianDate
    });
    from.setDate(new Date(1420, 1, 1));

    // Images Slider ..
    $(function () {
        $('#slider-steps, .slider-steps').slick({
            dots: false,
            rtl: true,
            slidesToShow: 1,
            slidesToScroll: 1,
            arrows: true,
            autoplay: false,
            nextArrow: document.getElementById('prev-step'),
            prevArrow: document.getElementById('next-step')
        });
    });

    // Ichiro input focus
    $(function () {
        $('.input--ichiro').each(function (i) {
            $(this).children('label').attr('for', `ichiro_el${i}`).siblings().attr('id', `ichiro_el${i}`);
        });
        $('.labelSwitch').each(function (i) {
            var target = $(this).attr('for');
            if ($(this).siblings(`#${target}`).length == 0) {
                $(this).attr('for', `switch_target${i}`).siblings(`.${target}`).attr('id', `switch_target${i}`);
            }
        });
    });

    // change selectbox value
    var selectOptionRange = function (element, range, first) {
        $(element).find('option').remove();
        switch (typeof (first)) {
            case 'object':
                $(element).append('<option value="' + first.val() + '" selected="selected" disabled="disabled">' + first.text() + '</option>');
                break;
            case 'string':
                $(element).append('<option selected="selected" disabled="disabled">' + first + '</option>');
                break;
            default:
                $(element).append('<option class="removeonfocus"></option>');
        }
        if (typeof (range['min']) != 'undefined') {
            for (var i = range['min']; i <= range['max']; i++) {
                $(element).append('<option value="' + i + '">' + i + '</option>');
            }
        } else {
            for (var i in range) {
                $(element).append('<option value="' + range[i] + '">' + range[i] + '</option>');
            }
        }
        return $(element);
    }

    $('select[option-range-min][option-range-max]').each(function () {
        selectOptionRange(
            this,
            {
                min: $(this).attr('option-range-min'),
                max: $(this).attr('option-range-max')
            },
            $(this).find('[selected]')
        );
    });

    // year selector
    $('.idnumber').on('change blur', function () {
        var yearRange = { min: 1900, max: gregorianDate.currentYear };
        var monthRange = gregorianDate.months;
        if ($.trim($(this).val()).substring(0, 1) == 1) {
            yearRange = { min: 1318, max: hijriDate.currentYear };
            monthRange = hijriDate.months;
        }
        selectOptionRange(
            $(this).closest('.idnumber_wrapper').find('.idnumber_month').select2('destroy'),
            monthRange,
            $(this).closest('.idnumber_wrapper').find('.idnumber_month option:selected').html()
        ).select2();
        selectOptionRange(
            $(this).closest('.idnumber_wrapper').find('.idnumber_year').select2('destroy'),
            yearRange,
            $(this).closest('.idnumber_wrapper').find('.idnumber_year option:selected').html()
        ).select2();
    });

    // show loading
    $('form').on('submit', function () {
        $('#page-loading').show();
    });

});
