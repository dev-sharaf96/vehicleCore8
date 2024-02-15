/* || ===

= IMKAN.com.sa
= APP FIRE

=== || */
(function () {

    function initFeatureSlider() {

        $('#slider.slides > div,#slider-steps .item').show();
         //// Featured Slider ..
         //$('#top-slider, .top-slider').slick({
         //    dots: true,
         //    rtl: true,
         //    slidesToShow: 1,
         //    slidesToScroll: 1,
         //    arrows: false,
         //    autoplay: true,
         //    autoplaySpeed: 5000,
         //    asNavFor: '.slides .slider',
         //    appendDots: $(".arrows-move")
         //});
        var $slider = $('.slides .slider');
        $slider.slick({
            dots: false,
            rtl: true,
             //asNavFor: '#top-slider, .top-slider',
            arrows: false,
            slidesToShow: 1,
            slidesToScroll: 1,
            autoplay: true,
            autoplaySpeed: 5000
        });
    }

    function scrollDownLink() {
        // ScroolDown Link
        $('#top-slider a,.scroll-down a').on('click', function (e) {
            e.preventDefault();
            if ($(this).attr('href') !== '#') {
              $('html, body').animate({
                scrollTop: $($(this).attr('href')).offset().top - ($('.tabs').outerHeight() + $('#top-page').outerHeight())
              }, 750, 'linear');
            }
        });
    }

    function initTabs() {
        // tabs
        $('.tabs li').on('click', function () {
            $('#' + $(this).data('link')).addClass('active').siblings().removeClass('active');
            $(this).removeClass('not-now').siblings().addClass('not-now');
            if(($('#' + $(this).data('link')).offset().top - ($('.tabs').outerHeight() + $('#top-page').outerHeight())).toFixed() != $('html, body').scrollTop()) {
              $('html, body').animate({
                  scrollTop: $('#' + $(this).data('link')).offset().top - ($('.tabs').outerHeight() + $('#top-page').outerHeight())
              }, 750, 'linear');
            }
        });
    }
    function initFocusBlur() {
        // Focus Blur
        $('.input__field').on('focus', function () {
            $(this).parent().addClass('input--filled');
        });
        $('.input__field').on('blur', function () {
            if (!$(this).val()) {
                $(this).parent().removeClass('input--filled');
            }
        });
    }
    function initPartners() {

        // Partners
        $('.img-list').slick({
            dots: false,
            rtl: true,
            slidesToShow: 7,
            arrows: true,
            autoplay: true,
            autoplaySpeed: 5000,
            nextArrow: document.getElementById('prev-comp'),
            prevArrow: document.getElementById('next-comp'),
            appendDots: $(".arrows-move"),
            swipeToSlide: true,
            infinite: true,
            responsive: [{
                breakpoint: 1024,
                settings: {
                    slidesToShow: 6
                }
            },
            {
                breakpoint: 600,
                settings: {
                    slidesToShow: 4
                }
            },
            {
                breakpoint: 480,
                settings: {
                    slidesToShow: 2,
                    arrows: false
                }
            }

            ]
        });
    }
    function initImageSlider() {

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
    }

    function initIchiro() {

        // Ichiro input focus
        $(function () {
                if ($('.input-element').val()) {
                    $(this).parent().addClass('active');
                }
            $('.input--ichiro').each(function (i) {
                $(this).children('label').attr('for', 'ichiro_el'+i+'').siblings().attr('id', 'ichiro_el'+i+'');
            });
            $('.labelSwitch').each(function (i) {
                var target = $(this).attr('for');
                if ($(this).siblings('#'+target+'').length == 0) {
                    $(this).attr('for', 'switch_target'+i+'').siblings('.'+target+'').attr('id', 'switch_target'+i+'');
                }
            });
            $('.input-element').on('focus', function () {
                $(this).parent().addClass('active');
              });
              $('.input-element').on('blur', function () {
                if (!$(this).val()) {
                    $(this).parent().removeClass('active');
                }
              });
        });
    }
    function initCarRegisteration() {
        // Car Registration ..
        $('#endcarregister, .endcarregister').each(function () {
            const $field = $(this);
            const endRegisterPicker = new Pikaday({
                field: $field.get(0),
                firstDay: 1,
                minDate: new Date($field.data('min-date')),
                maxDate: new Date($field.data('max-date')),
                format: 'D/M/YYYY',
                toString: function toString(date, format) {
                    // you should do formatting based on the passed format,
                    // but we will just return 'D/M/YYYY' for simplicity
                    const day = date.getDate();
                    const month = date.getMonth() + 1;
                    const year = date.getFullYear();
                    return ''+day+'/'+month+'/'+year+'';
                },
                parse: function parse(dateString, format) {
                    // dateString is the result of `toString` method
                    const parts = dateString.split('/');
                    const day = parseInt(parts[0], 10);
                    const month = parseInt((parts[1] - 1).toString(), 10);
                    const year = parseInt(parts[1], 10);
                    return new Date(year, month, day);
                },
            });
            endRegisterPicker.setDate(new Date());
        });
    }

    function anyNewFun(id) {
        $('#'+id+'').calendarsPicker({
            calendar: $.calendars.instance('islamic'),
            onSelect: function(dates) { 
                var minDate = dates[0]; 
                for (var i = 1; i < dates.length; i++) { 
                    if (dates[i].compareTo(minDate) == -1) { 
                        minDate = dates[i]; 
                    } 
                } 
                $('#dateHolder').val(minDate.formatDate()); 
            } 
        });
    }
    window.loadUi = function () {
        $(document).ready(function () {
            initFeatureSlider();
            scrollDownLink();
            initTabs();
            initFocusBlur();
            initPartners();
            initImageSlider();
            initIchiro();
            initCarRegisteration();
        });
    };
    // window.initDatePicker = function(id){
    //     $(document).ready(function () {
    //         anyNewFun(id);
    //     });
    // };
})();

