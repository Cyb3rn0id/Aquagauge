using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Drawing.Drawing2D;

namespace AquaControls
{
    /// <summary>
    /// Aqua Gauge Control - A Windows User Control.
    /// Author  : Ambalavanar Thirugnanam
    /// Date    : 24th August 2007
    /// email   : ambalavanar.thiru@gmail.com
    /// This is control is for free. You can use for any commercial or non-commercial purposes.
    /// 
    /// Corrected bugs and other features by 
    /// Bernardo Giovanni - http://www.settorezero.com
    /// settorezero@gmail.com
    /// see the attached changelog for a list of modifications
    /// 
    ///  
    /// [Please do no remove this header when using this control in your application.]
    /// </summary>
    public partial class AquaGauge : UserControl
    {
        #region Private Attributes
       
        private float minValue;
        private float maxValue;
        // private float threshold;
        private float currentValue;
        // private float recommendedValue;
        private int noOfDivisions;
        private int noOfSubDivisions;
        private string dialText;
        private Color dialColor = Color.MidnightBlue;
        private float glossinessAlpha = 40;
        private int oldWidth, oldHeight;
        int x, y, width, height;
        float fromAngle = 135F;
        float toAngle = 405F;
        private bool enableTransparentBackground;
        private bool requiresRedraw;
        private Image backgroundImg;
        private Rectangle rectImg;
        
        // following properties added by Bernardo Giovanni (http://www.settorezero.com)
        private Font dialTextFont;
        private Color rimColor = Color.Gold;
        private Color pointerColor = Color.Black;
        private Color dialBorderColor = Color.SlateGray;
        private Color scaleColor = Color.Gold;
        private Color digitalValueColor = Color.Orange;
        private Color digitalValueBackColor = Color.White;
        private Color dialTextColor = Color.Gold;
        private Color threshold1Color = Color.LawnGreen;
        private Color threshold2Color = Color.Red ;
        
        private bool digitalValueVisible;
        private bool valueToDigital;
        
        private float digitalValue;
        private float threshold1Start=0;
        private float threshold1Stop=0;
        private float threshold2Start=0;
        private float threshold2Stop=0;
        
        private int digitalValueDecimalPlaces;
        
        private int dialAlpha = 255;
        private int rimAlpha=255;
        private int scaleFontSizeDivider=22;
        private int dialTextVOffset = 0;
        private int digitalValueBackAlpha = 1;
        private int decimalPlaces;
                
        #endregion

        public AquaGauge()
        {
            InitializeComponent();
            x = 5;
            y = 5;
            this.Width = 200;
            this.Height = 200;
            width = this.Width - 10;
            height = this.Height - 10;
            this.digitalValueVisible = true;
            this.noOfDivisions = 10;
            this.noOfSubDivisions = 3;
            this.maxValue = 100;
            this.dialTextFont = this.Font;
            this.decimalPlaces = 0;
            this.digitalValueDecimalPlaces =0;
            this.ValueToDigital = true;
            this.SetStyle(ControlStyles.SupportsTransparentBackColor, true);
            this.SetStyle(ControlStyles.ResizeRedraw, true);
            this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            this.SetStyle(ControlStyles.UserPaint, true);
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer, true);                     
            this.BackColor = Color.Transparent;
            this.Resize += new EventHandler(AquaGauge_Resize);
            this.requiresRedraw = true;
        }

        #region Public Properties
        /// <summary>
        /// Start point for threshold arc n°1
        /// </summary>
        [Description("Start point for threshold arc n°1")]
        [DefaultValue("0")]
        // added by Bernardo Giovanni (http://www.settorezero.com)
        public float Threshold1Start
        {
        	get{return this.threshold1Start;}
        	set
        	{
        		if (value > maxValue)
        			value=maxValue;
        		if (value < minValue)
        			value=minValue;
        		this.threshold1Start = value;
        		requiresRedraw = true;
        		this.Invalidate();
        	}
        }
        
        /// <summary>
        /// End point for threshold arc n°1
        /// </summary>
        [Description("End point for threshold arc n°1")]
        [DefaultValue("0")]
        // added by Bernardo Giovanni (http://www.settorezero.com)
        public float Threshold1Stop
        {
        	get{return this.threshold1Stop;}
        	set
        	{
        		if (value > maxValue)
        			value=maxValue;
        		if (value < minValue)
        			value=minValue;
        		this.threshold1Stop = value;
        		requiresRedraw = true;
        		this.Invalidate();
        	}
        }
        
        /// <summary>
        /// Color for Threshold n°1
        /// </summary>
        [Description("Color for Threshold n°1")]
        // added by Bernardo Giovanni (http://www.settorezero.com)
        public Color Threshold1Color
        {
        	get{return this.threshold1Color;}
        	set
        	{
        		threshold1Color = value;
        		requiresRedraw = true;
        		this.Invalidate();
        	}
        }
        
        /// <summary>
        /// Start point for threshold arc n°2
        /// </summary>
        [Description("Start point for threshold arc n°2")]
        [DefaultValue("0")]
        // added by Bernardo Giovanni (http://www.settorezero.com)
   		public float Threshold2Start
        {
        	get{return this.threshold2Start;}
        	set
        	{
        		if (value > maxValue)
        			value=maxValue;
        		if (value < minValue)
        			value=minValue;
        		this.threshold2Start = value;
        		requiresRedraw = true;
        		this.Invalidate();
        	}
        }
        
   		/// <summary>
   		/// End point for threshold arc n°2
   		/// </summary>
   		[Description("End point for threshold arc n°2")]
   		[DefaultValue("0")]
   		// added by Bernardo Giovanni (http://www.settorezero.com)
   		public float Threshold2Stop
        {
        	get{return this.threshold2Stop;}
        	set
        	{
        		if (value > maxValue)
        			value=maxValue;
        		if (value < minValue)
        			value=minValue;
        		this.threshold2Stop = value;
        		requiresRedraw = true;
        		this.Invalidate();
        	}
        }
        
   		/// <summary>
   		/// Color for threshold arc n°2
   		/// </summary>
   		[Description("Color for threshold arc n°2")]
   		// added by Bernardo Giovanni
   		public Color Threshold2Color
        {
        	get{return this.threshold2Color;}
        	set
        	{
        		threshold2Color = value;
        		requiresRedraw = true;
        		this.Invalidate();
        	}
        }
         
        /// <summary>
        /// If TRUE, DigitalValue displays same value as the pointer. If FALSE digitalValue displays DigitalValue value
        /// </summary>
        [Description("If TRUE, DigitalValue displays same value as the pointer. If FALSE digitalValue displays DigitalValue value")]
        // Added by Bernardo Giovanni (http://www.settorezero.com)
        public bool ValueToDigital
        {
        	get{return this.valueToDigital;}
        	set
        	{
        		valueToDigital = value;
        		requiresRedraw = true;
        		this.Invalidate();
        	}
        	
        }
        
        /// <summary>
        /// Value used for DigitalValue if ValueToDigital is FALSE
        /// </summary>
        [Description("Value used for DigitalValue if ValueToDigital is FALSE")]
        [DefaultValue("0")]
        // Added by Bernardo Giovanni (http://www.settorezero.com)
        public float DigitalValue
        {
        	get{return this.digitalValue;}
        	set
        	{
        		digitalValue=value;
        		requiresRedraw = true;
        		this.Invalidate();
        	}
        }
        
        /// <summary>
        /// Set the decimal places used for digital values if ValueToDigital=FALSE. (0-2)
        /// </summary>
        [Description("Set the decimal places used for digital values if ValueToDigital=FALSE. (0-2)")]
        [DefaultValue("0")]
        // Added by Bernardo Giovanni (http://www.settorezero.com)
        public int DigitalValueDecimalPlaces
        {
        	get{return this.digitalValueDecimalPlaces;}
        	set
        	{
        		if (value<0)
        		{
        			value=0;
        		}
        		else if (value>2)
        		{
        			value=2;
        		}
        		digitalValueDecimalPlaces = value;
        		requiresRedraw = true;
        		this.Invalidate();
        	}
        }
        
        /// <summary>
        /// Vertical offset for dial text (text up if less than 0, text down if bigger than 0)
        /// </summary>
        [Description("Vertical offset for dial text (text up if less than 0, text down if bigger than 0)")]
        [DefaultValue("0")]
        // Added by Bernardo Giovanni (http://www.settorezero.com)
        public int DialTextVOffset
        {
        	get{return this.dialTextVOffset;}
        	set
        	{
        		this.dialTextVOffset=value;
        		requiresRedraw = true;
        		this.Invalidate();
        	}
        }
        
        /// <summary>
        /// Divider used to resize scale font (15-25, 15=font bigger, 25=font smaller)
        /// </summary>
        [Description("Divider used to resize scale font (15-25, 15=font bigger, 25=font smaller)")]
        [DefaultValue("23")]
        // Added by Bernardo Giovanni (http://www.settorezero.com)
        public int ScaleFontSizeDivider
        {
        	get{return this.scaleFontSizeDivider;}
        	set
        	{
        		if (value<15)
        		{
        			value=15;
        		}
        		else if (value >25)
        		{
        			value=25;
        		}
        		this.scaleFontSizeDivider = value;
        		requiresRedraw = true;
        		this.Invalidate();
        	}
        }
        
        /// <summary>
        /// Opacity of external rim (0-255)
        /// </summary>
        [Description("Opacity of external rim (0-255)")]
        [DefaultValue("255")]
        // Added by Bernardo Giovanni (http://www.settorezero.com)
        public int RimAlpha
        {
        	get{return this.rimAlpha;}
        	set
        	{
        		if (value<0)
        		{
        			value=0;
        		}
        		else if (value>255)
        		{
        			value=255;
        		}
        		this.rimAlpha = value;
        		requiresRedraw = true;
        		this.Invalidate();
        	}
        }
        
        /// <summary>
        /// Opacity of gauge background (0-255)
        /// </summary>
        [Description("Opacity of gauge background (0-255)")]
        [DefaultValue("255")]
        // Added by Bernardo Giovanni (http://www.settorezero.com)
        public int DialAlpha
        {
        	get{return this.dialAlpha;}
        	set
        	{
        		if (value<0)
        		{
        			value=0;
        		}
        		else if (value>255)
        		{
        			value=255;
        		}
        		this.dialAlpha = value;
        		requiresRedraw = true;
        		this.Invalidate();
        	}
        }
        
        /// <summary>
        /// Decimal places for scale (and digital value if ValueToDigital=TRUE) (0-2)
        /// </summary>
        [Description("Decimal places for scale (and digital value if ValueToDigital=TRUE) (0-2)")]
        [DefaultValue("0")]
        // Added by Bernardo Giovanni (http://www.settorezero.com)
        public int DecimalPlaces
        {
        	get{return this.decimalPlaces;}
        	set
        	{
        		if (value<0) 
        		{
        			value=0;
        		}
        		else if (value>2)
        		{
        			value=2;
        		}
        		this.decimalPlaces = value;
        		requiresRedraw = true;
        		this.Invalidate();
        	}
        }
        
        /// <summary>
        /// Opacity of digital value backcolor (0-255)
        /// </summary>
        [Description("Opacity of digital value backcolor (0-255)")]
        [DefaultValue("50")]
        // Added by Bernardo Giovanni (http://www.settorezero.com)
        public int DigitalValueBackAlpha
        {
        	get{return this.digitalValueBackAlpha;}
        	set
        	{
        		if (value>255)
        		{
        			value=255;
        		}
        		else if (value<0)
        		{
        			value=0;
        		}
        		this.digitalValueBackAlpha=value;
        		requiresRedraw = true;
        		this.Invalidate();
        	}
        }
        
        /// <summary>
        /// Dial Text Color
        /// </summary>
        [Description("Dial Text Color")]
        // Added by Bernardo Giovanni (http://www.settorezero.com)
        public Color DialTextColor
        {
        	get{return this.dialTextColor;}
        	set
        		{
        		this.dialTextColor = value;
        		requiresRedraw = true;
        		this.Invalidate();
        		}
        }
        
        /// <summary>
        /// Digital Value Background color
        /// </summary>
        [Description("Digital Value Background color")]
        // Added by Bernardo Giovanni (http://www.settorezero.com)
        public Color DigitalValueBackColor
        {
        	get{return this.digitalValueBackColor;}
        	set
        		{
        		this.digitalValueBackColor = value;
        		requiresRedraw = true;
        		this.Invalidate();
        		}
        }
        
        /// <summary>
        /// Digital Value Color
        /// </summary>
        [Description("Digital Value Color")]
        // Added by Bernardo Giovanni (http://www.settorezero.com)
        public Color DigitalValueColor
        {
        	get {return this.digitalValueColor;}
        	set
        	{
        		this.digitalValueColor = value;
        		requiresRedraw = true;
        		this.Invalidate();
        	}
        }
        
        /// <summary>
        /// Dial text font
        /// </summary>
        [Description("Dial text font")]
        // Added by Bernardo Giovanni (http://www.settorezero.com)
        public Font DialTextFont
        {
        	get {return this.dialTextFont;}
        	set
        	{
        		this.dialTextFont = value;
        		requiresRedraw = true;
        		this.Invalidate();
        	}
        }
        
        
        /// <summary>
        /// Scale color
        /// </summary>
        [Description("Scale Color")]
        // Added by Bernardo Giovanni (http://www.settorezero.com)
        public Color ScaleColor
        {
        	get {return this.scaleColor;}
        	set
        	{
        		scaleColor=value;
        		requiresRedraw = true;
        		this.Invalidate();
        	}
        }
        
        /// <summary>
        /// Dial Border Color
        /// </summary>
        [Description("Dial Border Color")] 
        // Added by Bernardo Giovanni (http://www.settorezero.com)
     	public Color DialBorderColor
     	{
     		get {return this.dialBorderColor;}
     		set
     		{
     			dialBorderColor = value;
     			requiresRedraw = true;
     			this.Invalidate();
     		}
     	}
     	
        /// <summary>
        /// Pointer Color
        /// </summary>
        [Description("Pointer Color")]
        // Added by Bernardo Giovanni (http://www.settorezero.com)
        public Color PointerColor
        {
        	get {return this.pointerColor;}
        	set
        	{
        		pointerColor=value;
        		requiresRedraw = true;
        		this.Invalidate();
        	}
        }
        
        /// <summary>
        /// External Rim Color
        /// </summary>
        [Description("External Rim Color")]
        // Added by Bernardo Giovanni (http://www.settorezero.com)
        public Color RimColor
        {
        	get {return this.rimColor;}
        	set
        	{
        		rimColor = value;
        		requiresRedraw=true;
        		this.Invalidate();
        	}
        }
         
        /// <summary>
        /// Show/Hide digital value
        /// </summary>
        [DefaultValue(true)]
        [Description("Show/Hide digital value")]
        // Added by Bernardo Giovanni (http://www.settorezero.com)
        public bool DigitalValueVisible
        {
        	get { return this.digitalValueVisible ;}
        	set
        	{
        		digitalValueVisible = value ;
        		requiresRedraw = true;
        		this.Invalidate();
        	}
        	
        }
        
        /// <summary>
        /// Mininum value on the scale
        /// </summary>
        [DefaultValue(0)]
        [Description("Mininum value on the scale")]
        public float MinValue
        {
            get { return minValue; }
            set
            {
                if (value < maxValue)
                {
                    minValue = value;
                    if (currentValue < minValue)
                        currentValue = minValue;
                    if (threshold1Start < minValue)
                    	threshold1Start = minValue;
                    if (threshold1Stop < minValue)
                    	threshold1Stop = minValue;
                    if (threshold2Start < minValue)
                    	threshold2Start = minValue;
                    if (threshold2Stop < minValue)
                    	threshold2Stop = minValue;
                    /*
                    if (recommendedValue < minValue)
                        recommendedValue = minValue;
                    */
                    requiresRedraw = true;
                    this.Invalidate();
                }
            }
        }

        /// <summary>
        /// Maximum value on the scale
        /// </summary>
        [DefaultValue(100)]
        [Description("Maximum value on the scale")]
        public float MaxValue
        {
            get { return maxValue; }
            set
            {
                if (value > minValue)
                {
                    maxValue = value;
                    if (currentValue > maxValue)
                        currentValue = maxValue;
                    if (threshold1Start > maxValue)
                    	threshold1Start = maxValue;
                    if (threshold1Stop > maxValue)
                    	threshold1Stop = maxValue;
                    if (threshold2Start > maxValue)
                    	threshold2Start = maxValue;
                    if (threshold2Stop > maxValue)
                    	threshold2Stop = maxValue;
                    /*
                    if (recommendedValue > maxValue)
                        recommendedValue = maxValue;
                    */
                    requiresRedraw = true;
                    this.Invalidate();
                }
            }
        }

        /*
        /// <summary>
        /// Gets or Sets the Threshold area from the Recommended Value. (1-99%)
        /// </summary>
        [DefaultValue(25)]
        [Description("Gets or Sets the Threshold area from the Recommended Value. (1-99%)")]
        public float ThresholdPercent
        {
            get { return threshold; }
            set
            {
                if (value > 0 && value < 100)
                {
                    threshold = value;
                    requiresRedraw = true;
                    this.Invalidate();
                }
            }
        }
		*/
		
		/*
        /// <summary>
        /// Threshold value from which green area will be marked.
        /// </summary>
        [DefaultValue(25)]
        [Description("Threshold value from which green area will be marked.")]
        public float RecommendedValue
        {
            get { return recommendedValue; }
            set
            {
                if (value > minValue && value < maxValue) 
                {
                    recommendedValue = value;
                    requiresRedraw = true;
                    this.Invalidate();
                }
            }
        }
		*/
		
        /// <summary>
        /// Value where the pointer will point to.
        /// </summary>
        [DefaultValue(0)]
        [Description("Value where the pointer will point to.")]
        public float Value
        {
            get { return currentValue; }
            set
            {
                if (value >= minValue && value <= maxValue)
                {
                    currentValue = value;
                    this.Refresh();
                }
            }
        }

        /// <summary>
        /// Background color of the dial
        /// </summary>
        [Description("Background color of the dial")]
        public Color DialColor
        {
            get { return dialColor; }
            set
            {
                dialColor = value;
                requiresRedraw = true;
                this.Invalidate();
            }
        }

        /// <summary>
        /// Glossiness strength. Range: 0-255
        /// </summary>
        [DefaultValue(40)]
        [Description("Glossiness strength. Range: 0-255")]
        public float Glossiness
        {
            get
            {
                return glossinessAlpha;
            }
            set
            {
            	if (value<0)
            	{
            		value=0;
            	}
            	else if (value>255)
            	{
            		value=255;
            	}
                glossinessAlpha=value;
                this.Refresh();
            }
        }

        /// <summary>
        /// Get or Sets the number of Divisions in the dial scale.
        /// </summary>
        [DefaultValue(10)]
        [Description("Get or Sets the number of Divisions in the dial scale.")]
        public int NoOfDivisions
        {
            get { return this.noOfDivisions; }
            set
            {
                if (value > 1 && value < 25)
                {
                    this.noOfDivisions = value;
                    requiresRedraw = true;
                    this.Invalidate();
                }
            }
        }

        /// <summary>
        /// Gets or Sets the number of Sub Divisions in the scale per Division.
        /// </summary>
        [DefaultValue(3)]
        [Description("Gets or Sets the number of Sub Divisions in the scale per Division.")]
        public int NoOfSubDivisions
        {
            get { return this.noOfSubDivisions; }
            set
            {
                if (value > 0 && value <= 10)
                {
                    this.noOfSubDivisions = value;
                    requiresRedraw = true;
                    this.Invalidate();
                }
            }
        }

        /// <summary>
        /// Gets or Sets the Text to be displayed in the dial
        /// </summary>
        [Description("Gets or Sets the Text to be displayed in the dial")]
        public string DialText
        {
            get { return this.dialText; }
            set
            {
                this.dialText = value;
                requiresRedraw = true;
                this.Invalidate();
            }
        }

        /// <summary>
        /// Enables or Disables Transparent Background color.
        /// Note: Enabling this will reduce the performance and may make the control flicker.
        /// </summary>
        [DefaultValue(false)]
        [Description("Enables or Disables Transparent Background color. Note: Enabling this will reduce the performance and may make the control flicker.")]
        public bool EnableTransparentBackground
        {
            get { return this.enableTransparentBackground; }
            set
            {
                this.enableTransparentBackground = value;
                this.SetStyle(ControlStyles.OptimizedDoubleBuffer, !enableTransparentBackground);
                requiresRedraw = true;
                this.Refresh();  
            }
        }
        #endregion

        #region Overriden Control methods
               
        /// <summary>
        /// Draws the pointer.
        /// </summary>
        /// <param name="e"></param>
        // Edited on 29-12-2010 by Bernardo Giovanni (http://www.settorezero.com)
        // The digital value now is updated into this routine
        protected override void OnPaint(PaintEventArgs e)
        {
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            width = this.Width - x*2;
            height = this.Height - y*2;
            DrawPointer(e.Graphics, ((width) / 2) + x, ((height) / 2) + y);
            
            // Draws Digital Value if requested
            if (this.digitalValueVisible)
                	{
            		DrawDigitalValue (e.Graphics);
            		}
        }
                
        /// <summary>
        /// Draws the dial background.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnPaintBackground(PaintEventArgs e)
        {
            if (!enableTransparentBackground)
            {
                base.OnPaintBackground(e);
            }         
            
            e.Graphics.SmoothingMode = SmoothingMode.HighQuality;
            e.Graphics.FillRectangle(new SolidBrush(Color.Transparent), new Rectangle(0,0,Width,Height));
            if (backgroundImg == null || requiresRedraw)
            {
                backgroundImg = new Bitmap(this.Width, this.Height);
                Graphics g = Graphics.FromImage(backgroundImg);
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                width = this.Width - x * 2;
                height = this.Height - y * 2;
                rectImg = new Rectangle(x, y, width, height);

                //Draw background color
                Brush backGroundBrush = new SolidBrush(Color.FromArgb(dialAlpha, dialColor));
                if (enableTransparentBackground && this.Parent != null)
                	{
                    float gg = width / 60;
                    //g.FillEllipse(new SolidBrush(this.Parent.BackColor), -gg, -gg, this.Width+gg*2, this.Height+gg*2);
               		}
                g.FillEllipse(backGroundBrush, x, y, width, height);

                // Draw external border
                // edited on 29-12-2010 By Bernardo Giovanni (http://www.settorezero.com)
                // changed Color.SlateGray with dialBorderColor
                SolidBrush outlineBrush = new SolidBrush(Color.FromArgb(100, dialBorderColor));
                Pen outline = new Pen(outlineBrush, (float)(width * .03));
                g.DrawEllipse(outline, rectImg);
                Pen darkRim = new Pen(dialBorderColor);
                g.DrawEllipse(darkRim, x, y, width, height);

                //Draw scale
                DrawCalibration(g, rectImg, ((width) / 2) + x, ((height) / 2) + y);

                //Draw Colored Rim
                //edited on 29-12-2010 By Bernardo Giovanni (http://www.settorezero.com)
                //changed Color.SlateGray with dialBorderColor
                //changed alpha from 190 to 255
                Pen colorPen = new Pen(Color.FromArgb(rimAlpha, rimColor), this.Width / 40);
                //Pen blackPen = new Pen(Color.FromArgb(250, Color.black), this.Width / 200); // not used?!
                int gap = (int)(this.Width * 0.03F);
                Rectangle rectg = new Rectangle(rectImg.X + gap, rectImg.Y + gap, rectImg.Width - gap * 2, rectImg.Height - gap * 2);
                g.DrawArc(colorPen, rectg, 134, 272); // 135, 270
				
                // Thresholds
                float propAngle = (toAngle - fromAngle)/(MaxValue-MinValue); // proportion between angle and units
                float arcstart,arcsweep;
                
                // Draws second threshold
                if ((threshold2Stop - threshold2Start ) > 0)
                	{
                	colorPen = new Pen(Color.FromArgb(255, threshold2Color), this.Width / 40); // original value : this.width/30
                	rectg = new Rectangle(rectImg.X + gap, rectImg.Y + gap, rectImg.Width - gap * 2, rectImg.Height - gap * 2);
                	arcstart = (135 + ((threshold2Start-minValue) * propAngle)); // start angle
                	arcsweep = ((threshold2Stop-threshold2Start) * propAngle); // sweep (end point angle=start+sweep)
                	if (arcstart + arcsweep > 405) arcsweep = 405 - arcstart;
                	g.DrawArc(colorPen, rectg, arcstart, arcsweep);
                	}
                
                // Draws first threshold
                if ((threshold1Stop - threshold1Start ) > 0)
                	{
                	colorPen = new Pen(Color.FromArgb(255, threshold1Color), this.Width / 40); // original value : this.width/30
                	rectg = new Rectangle(rectImg.X + gap, rectImg.Y + gap, rectImg.Width - gap * 2, rectImg.Height - gap * 2);
                   	arcstart = (135 + ((threshold1Start-minValue) * propAngle)); // start angle
                	arcsweep = ((threshold1Stop-threshold1Start) * propAngle); // sweep (end point angle=start+sweep)
                	if (arcstart + arcsweep > 405) arcsweep = 405 - arcstart;
                	g.DrawArc(colorPen, rectg, arcstart, arcsweep);
                	}
                
                
                
                //Draw Threshold
                //Thresolds painted with the 6 new properties
                /*
                colorPen = new Pen(Color.FromArgb(255, Color.LawnGreen), this.Width / 30);
                rectg = new Rectangle(rectImg.X + gap, rectImg.Y + gap, rectImg.Width - gap * 2, rectImg.Height - gap * 2);
                float val = MaxValue - MinValue;
                val = (100 * (this.recommendedValue - MinValue)) / val;
                val = ((toAngle - fromAngle) * val) / 100;
                val += fromAngle;
                float stAngle = val - ((270 * threshold) / 200);
                if (stAngle < 135) stAngle = 135;
                float sweepAngle = ((270 * threshold) / 100);
                if (stAngle + sweepAngle > 405) sweepAngle = 405 - stAngle;
                g.DrawArc(colorPen, rectg, stAngle, sweepAngle);
                */
                
                //Draw digital value
                //Edited on 29-12-2010 By Bernardo Giovanni (http://www.settorezero.com)
                //digital value now is painted in OnPaint Method!
                //RectangleF digiRect = new RectangleF((float)this.Width / 2F - (float)this.width / 5F, (float)this.height / 1.2F, (float)this.width / 2.5F, (float)this.Height / 9F);
               	//RectangleF digiFRect = new RectangleF(this.Width / 2 - this.width / 7, (int)(this.height / 1.18), this.width / 4, this.Height / 12);
               	//g.FillRectangle(new SolidBrush(Color.FromArgb(30, Color.Gray)), digiRect);
               	//DisplayNumber(g, this.currentValue, digiFRect);

               	//Draw Dial Text
               	//Edited on 29-12-2010 By Bernardo Giovanni (http://www.settorezero.com)
                // changed this.forecolor with dialTextColor
                if (DialText != "")
                	{
                	// as subjested by user mythzxp on codeproject
                	StringFormat drawFormat = new StringFormat();
					drawFormat.Alignment = StringAlignment.Center;
					drawFormat.LineAlignment = StringAlignment.Center;
                	SizeF textSize = g.MeasureString(this.dialText, dialTextFont);
                	//RectangleF digiFRectText = new RectangleF(this.Width / 2 - textSize.Width / 2, ((int)(this.height / 1.5))+8, textSize.Width, textSize.Height);
                	//g.DrawString(dialText, dialTextFont, new SolidBrush(dialTextColor), digiFRectText);
                	RectangleF digiFRectText = new RectangleF(this.Width / 2 - textSize.Width / 2, (int)(this.height / 1.45)+dialTextVOffset, textSize.Width, textSize.Height+1);
                	g.DrawString(dialText, dialTextFont, new SolidBrush(dialTextColor), digiFRectText, drawFormat);
                	}
                
                requiresRedraw = false;
            }
            e.Graphics.DrawImage(backgroundImg, rectImg);
        }

        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.ExStyle |= 0x20;
                return cp;
            }
        }
        #endregion

        #region Private methods
        
        /// <summary>
        /// Draws digital value
        /// </summary>
        /// <param name="gr"></param>
        // Added on 29-12-2010 by Bernardo Giovanni (http://www.settorezero.com)
        private void DrawDigitalValue(Graphics gr)
        {
        	RectangleF digiRect = new RectangleF((float)this.Width / 2F - (float)this.width / 5F , ((float)this.height / 1.2F )- 6, (float)this.width / 2.5F , ((float)this.Height / 9F) - 2);
        	RectangleF digiFRect = new RectangleF(this.Width / 2 - this.width / 7, ((int)(this.height / 1.18)) - 6, this.width / 4, (this.Height / 12) - 2);
            gr.FillRectangle(new SolidBrush(Color.FromArgb(digitalValueBackAlpha, digitalValueBackColor)), digiRect);
            
            if (valueToDigital)// uses same value for pointer and digital value
            	{
            	DisplayNumber(gr, this.currentValue, digiFRect);
            	}
           	else // digital value will use his own value as defined by DigitalValue property
           		{
           		DisplayNumber(gr, this.digitalValue, digiFRect);
           		}
        }
        
        /// <summary>
        /// Draws the Pointer.
        /// </summary>
        /// <param name="gr"></param>
        /// <param name="cx"></param>
        /// <param name="cy"></param>
        private void DrawPointer(Graphics gr, int cx, int cy)
        // edited on 29-12-2010 by Bernardo Giovanni (http://www.settorezero.com)
        // changed Color.Black with pointerColor
        {
            float radius = this.Width / 2 - (this.Width * .12F);
            float val = MaxValue - MinValue;

            Image img = new Bitmap(this.Width, this.Height);
            Graphics g = Graphics.FromImage(img);
            g.SmoothingMode = SmoothingMode.AntiAlias;

            val = (100 * (this.currentValue - MinValue)) / val;
            val = ((toAngle - fromAngle) * val) / 100;
            val += fromAngle;

            float angle = GetRadian(val);
            float gradientAngle = angle;

            PointF[] pts = new PointF[5];

            pts[0].X = (float)(cx + radius * Math.Cos(angle));
            pts[0].Y = (float)(cy + radius * Math.Sin(angle));

            pts[4].X = (float)(cx + radius * Math.Cos(angle - 0.02));
            pts[4].Y = (float)(cy + radius * Math.Sin(angle - 0.02));

            angle = GetRadian((val + 20));
            pts[1].X = (float)(cx + (this.Width * .09F) * Math.Cos(angle));
            pts[1].Y = (float)(cy + (this.Width * .09F) * Math.Sin(angle));

            pts[2].X = cx;
            pts[2].Y = cy;

            angle = GetRadian((val - 20));
            pts[3].X = (float)(cx + (this.Width * .09F) * Math.Cos(angle));
            pts[3].Y = (float)(cy + (this.Width * .09F) * Math.Sin(angle));

            Brush pointer = new SolidBrush(pointerColor);
            g.FillPolygon(pointer, pts);

            PointF[] shinePts = new PointF[3];
            angle = GetRadian(val);
            shinePts[0].X = (float)(cx + radius * Math.Cos(angle));
            shinePts[0].Y = (float)(cy + radius * Math.Sin(angle));

            angle = GetRadian(val + 20);
            shinePts[1].X = (float)(cx + (this.Width * .09F) * Math.Cos(angle));
            shinePts[1].Y = (float)(cy + (this.Width * .09F) * Math.Sin(angle));

            shinePts[2].X = cx;
            shinePts[2].Y = cy;

            LinearGradientBrush gpointer = new LinearGradientBrush(shinePts[0], shinePts[2], Color.SlateGray, pointerColor);
            g.FillPolygon(gpointer, shinePts);

            Rectangle rect = new Rectangle(x, y, width, height);
            DrawCenterPoint(g, rect, ((width) / 2) + x, ((height) / 2) + y);

            DrawGloss(g);

            gr.DrawImage(img, 0, 0);
        }

        /// <summary>
        /// Draws the glossiness.
        /// </summary>
        /// <param name="g"></param>
        private void DrawGloss(Graphics g)
        {
            RectangleF glossRect = new RectangleF(
               x + (float)(width * 0.10),
               y + (float)(height * 0.07),
               (float)(width * 0.80),
               (float)(height * 0.7));
            LinearGradientBrush gradientBrush =
                new LinearGradientBrush(glossRect,
                Color.FromArgb((int)glossinessAlpha, Color.White),
                Color.Transparent,
                LinearGradientMode.Vertical);
            g.FillEllipse(gradientBrush, glossRect);

            //TODO: Gradient from bottom
            // added : -5 to fit lower gloss on on digitalvalue
            glossRect = new RectangleF(
               x + (float)(width * 0.25),
               y + (float)(height * 0.77)-5,
               (float)(width * 0.50),
               (float)(height * 0.2)-2);
            int gloss = (int)(glossinessAlpha / 3);
            gradientBrush =
                new LinearGradientBrush(glossRect,
                Color.Transparent, Color.FromArgb(gloss, this.BackColor),
                LinearGradientMode.Vertical);
            g.FillEllipse(gradientBrush, glossRect);
        }

        /// <summary>
        /// Draws the center point.
        /// </summary>
        /// <param name="g"></param>
        /// <param name="rect"></param>
        /// <param name="cX"></param>
        /// <param name="cY"></param>
        // edited on 29-12-2010 by Bernardo Giovanni
        // changed Color.Black with pointerColor
        private void DrawCenterPoint(Graphics g, Rectangle rect, int cX, int cY)
        {
            float shift = Width / 5;
            RectangleF rectangle = new RectangleF(cX - (shift / 2), cY - (shift / 2), shift, shift);
            LinearGradientBrush brush = new LinearGradientBrush(rect, pointerColor, Color.FromArgb(100,this.dialColor), LinearGradientMode.Vertical);
            g.FillEllipse(brush, rectangle);

            shift = Width / 7;
            rectangle = new RectangleF(cX - (shift / 2), cY - (shift / 2), shift, shift);
            brush = new LinearGradientBrush(rect, Color.SlateGray, pointerColor, LinearGradientMode.ForwardDiagonal);
            g.FillEllipse(brush, rectangle);
        }
		
        /// <summary>
        /// Returns a string used to format the decimal numbers (eg. '0.0000','00.000' etc.)
        /// </summary>
        /// <param name="dp">Decimal places</param>
        /// <returns></returns>
        // Added by Bernardo Giovanni (http://www.settorezero.com)
        // used to format digital value
        public string DecimalFormat(int dp)
        {
        string str="00000";
        	switch(dp)
            	{
            		case 1:
            			str="0000.0";
            			break;
            		case 2:
            			str="000.00";
            			break;
            		case 3:
            			str="00.000";
            			break;
            		case 4:
            			str="0.0000";
            			break;
            	}
        return str;
        }
        
        /// <summary>
        /// Draws the scale
        /// </summary>
        /// <param name="g"></param>
        /// <param name="rect"></param>
        /// <param name="cX"></param>
        /// <param name="cY"></param>
        // Edit on 30-12-2010 by Bernardo Giovanni (http://www.settorezero.com)
        // Added capability to show numbers in decimal format using new DecimalPlaces property
        private void DrawCalibration(Graphics g, Rectangle rect, int cX, int cY)
        {
            int noOfParts = this.noOfDivisions + 1;
            int noOfIntermediates = this.noOfSubDivisions;
            float currentAngle = GetRadian(fromAngle);
            int gap = (int)(this.Width * 0.01F);
            float shift = this.Width / 25;
            Rectangle rectangle = new Rectangle(rect.Left + gap, rect.Top + gap, rect.Width - gap, rect.Height - gap);
                                   
            float x,y,x1,y1,tx,ty,radius;
            radius = rectangle.Width/2 - gap*5;
            float totalAngle = toAngle - fromAngle;
            float incr = GetRadian(((totalAngle) / ((noOfParts - 1) * (noOfIntermediates + 1))));
            
            Pen thickPen = new Pen(scaleColor, Width/50);
            Pen thinPen = new Pen(scaleColor, Width/100);
            float rulerValue = MinValue;
                        
            for (int i = 0; i <= noOfParts; i++)
            {
                //Draw Thick Line
                x = (float)(cX + radius * Math.Cos(currentAngle));
                y = (float)(cY + radius * Math.Sin(currentAngle));
                x1 = (float)(cX + (radius - Width/20) * Math.Cos(currentAngle));
                y1 = (float)(cY + (radius - Width/20) * Math.Sin(currentAngle));
                g.DrawLine(thickPen, x, y, x1, y1);
                
                //Draw Strings
                
                // Select format for decimal numbers
                string str="N0";
                switch(decimalPlaces)
                {
                	case 1:
                		str = "N1";
                		break;
                	case 2:
                		str = "N2";
                		break;
                }
                
                int offset=10;
                if (this.decimalPlaces>0)
                {
                	offset=8;
                }
               
                tx = (float)(cX + (radius - Width / offset) * Math.Cos(currentAngle));
                ty = (float)(cY-shift + (radius - Width / 10) * Math.Sin(currentAngle));
                                
                Brush stringPen = new SolidBrush(scaleColor); // this.ForeColor);
                
                StringFormat strFormat = new StringFormat(StringFormatFlags.NoClip);
                strFormat.Alignment = StringAlignment.Center;
                // width was: this.Width / 23
                // now from this.width / 15 to this.width / 25
                Font f = new Font(this.Font.FontFamily, (float)(this.Width / scaleFontSizeDivider), this.Font.Style);
                g.DrawString(rulerValue.ToString(str) + "", f, stringPen, new PointF(tx, ty), strFormat);
                
                rulerValue += (float)((MaxValue - MinValue) / (noOfParts - 1));
                rulerValue = (float)Math.Round(rulerValue, 2);
                              
                //currentAngle += incr;
                if (i == noOfParts -1)
                    break;
                for (int j = 0; j <= noOfIntermediates; j++)
                {
                    //Draw thin lines 
                    currentAngle += incr;
                    x = (float)(cX + radius * Math.Cos(currentAngle));
                    y = (float)(cY + radius * Math.Sin(currentAngle));
                    x1 = (float)(cX + (radius - Width/50) * Math.Cos(currentAngle));
                    y1 = (float)(cY + (radius - Width/50) * Math.Sin(currentAngle));
                    g.DrawLine(thinPen, x, y, x1, y1);                    
                }
            }
        }

        /// <summary>
        /// Converts the given degree to radian.
        /// </summary>
        /// <param name="theta"></param>
        /// <returns></returns>
        public float GetRadian(float theta)
        {
            return theta * (float)Math.PI / 180F;
        }

        /// <summary>
        /// Displays the given number in the 7-Segment format.
        /// </summary>
        /// <param name="g"></param>
        /// <param name="number"></param>
        /// <param name="drect"></param>
        // Edited on 29-12-2010 by Bernardo Giovanni (http://www.settorezero.com)
        // corrected bug: decimal values not correctly displayed on system using
        // comma as decimal separator
        private void DisplayNumber(Graphics g, float number, RectangleF drect)
        {
            try
            {
            	/* Added decimal places capability.
            	 * If ValueToDigital = TRUE, digitalValue use same decimal places as "DecimalPlaces"
            	 * if ValueToDigital = FALSE, digitalValue use his own property "DigitalValueDecimalPlaces"
            	 */
         
            	int decPla=0;
            	if (this.ValueToDigital)
            		{
            		decPla=this.decimalPlaces;
            		}
            	else
            		{
            		decPla=this.digitalValueDecimalPlaces;
            		}
            	string num = number.ToString(DecimalFormat(decPla));
                num.PadLeft(5-decPla , '0');
                float shift = 0; 
                if (number < 0)
                {
                    shift -= width/17;
                }
                bool drawDPS = false;
                char[] chars = num.ToCharArray();
                for (int i = 0; i < chars.Length; i++)
                {
                    char c = chars[i];
                    // added: || (chars[i + 1] == ',')
                    if (i < chars.Length - 1 && ((chars[i + 1] == '.') || (chars[i + 1] == ',')))
                        drawDPS = true;
                    else
                        drawDPS = false;
                    // added: && (c != ',')
                    if ((c != '.') && (c != ','))
                    {
                        if (c == '-')
                        {
                            DrawDigit(g, -1, new PointF(drect.X + shift, drect.Y), drawDPS, drect.Height);
                        }
                        else
                        {
                            DrawDigit(g, Int16.Parse(c.ToString()), new PointF(drect.X + shift, drect.Y), drawDPS, drect.Height);
                        }
                        shift += 15 * this.width / 250;
                    }
                    else
                    {
                        shift += 2 * this.width / 250;
                    }
                }
            }
            catch (Exception)
            {
            }
        }

        /// <summary>
        /// Draws a digit in 7-Segment format.
        /// </summary>
        /// <param name="g"></param>
        /// <param name="number"></param>
        /// <param name="position"></param>
        /// <param name="dp"></param>
        /// <param name="height"></param>
        // Edited on 29-12-2010 by Bernardo Giovanni (http://www.settorezero.com)
        // added capability to choose the color
        private void DrawDigit(Graphics g, int number, PointF position, bool dp, float height)
        {
            float width;
            width = 10F * height/13;
            
            Pen outline = new Pen(Color.FromArgb(20, this.dialColor));
            Pen fillPen = new Pen(digitalValueColor);

            #region Form Polygon Points
            //Segment A
            PointF[] segmentA = new PointF[5];
            segmentA[0] = segmentA[4] = new PointF(position.X + GetX(2.8F, width), position.Y + GetY(1F, height));
            segmentA[1] = new PointF(position.X + GetX(10, width), position.Y + GetY(1F, height));
            segmentA[2] = new PointF(position.X + GetX(8.8F, width), position.Y + GetY(2F, height));
            segmentA[3] = new PointF(position.X + GetX(3.8F, width), position.Y + GetY(2F, height));            

            //Segment B
            PointF[] segmentB = new PointF[5];
            segmentB[0] = segmentB[4] = new PointF(position.X + GetX(10, width), position.Y + GetY(1.4F, height));
            segmentB[1] = new PointF(position.X + GetX(9.3F, width), position.Y + GetY(6.8F, height));
            segmentB[2] = new PointF(position.X + GetX(8.4F, width), position.Y + GetY(6.4F, height));
            segmentB[3] = new PointF(position.X + GetX(9F, width), position.Y + GetY(2.2F, height)); 

            //Segment C
            PointF[] segmentC = new PointF[5];
            segmentC[0] = segmentC[4] = new PointF(position.X + GetX(9.2F, width), position.Y + GetY(7.2F, height));
            segmentC[1] = new PointF(position.X + GetX(8.7F, width), position.Y + GetY(12.7F, height));
            segmentC[2] = new PointF(position.X + GetX(7.6F, width), position.Y + GetY(11.9F, height));
            segmentC[3] = new PointF(position.X + GetX(8.2F, width), position.Y + GetY(7.7F, height)); 

            //Segment D
            PointF[] segmentD = new PointF[5];
            segmentD[0] = segmentD[4] = new PointF(position.X + GetX(7.4F, width), position.Y + GetY(12.1F, height));
            segmentD[1] = new PointF(position.X + GetX(8.4F, width), position.Y + GetY(13F, height));
            segmentD[2] = new PointF(position.X + GetX(1.3F, width), position.Y + GetY(13F, height));
            segmentD[3] = new PointF(position.X + GetX(2.2F, width), position.Y + GetY(12.1F, height)); 

            //Segment E
            PointF[] segmentE = new PointF[5];
            segmentE[0] = segmentE[4] = new PointF(position.X + GetX(2.2F, width), position.Y + GetY(11.8F, height));
            segmentE[1] = new PointF(position.X + GetX(1F, width), position.Y + GetY(12.7F, height));
            segmentE[2] = new PointF(position.X + GetX(1.7F, width), position.Y + GetY(7.2F, height));
            segmentE[3] = new PointF(position.X + GetX(2.8F, width), position.Y + GetY(7.7F, height)); 

            //Segment F
            PointF[] segmentF = new PointF[5];
            segmentF[0] = segmentF[4] = new PointF(position.X + GetX(3F, width), position.Y + GetY(6.4F, height));
            segmentF[1] = new PointF(position.X + GetX(1.8F, width), position.Y + GetY(6.8F, height));
            segmentF[2] = new PointF(position.X + GetX(2.6F, width), position.Y + GetY(1.3F, height));
            segmentF[3] = new PointF(position.X + GetX(3.6F, width), position.Y + GetY(2.2F, height));

            //Segment G
            PointF[] segmentG = new PointF[7];
            segmentG[0] = segmentG[6] = new PointF(position.X + GetX(2F, width), position.Y + GetY(7F, height));
            segmentG[1] = new PointF(position.X + GetX(3.1F, width), position.Y + GetY(6.5F, height));
            segmentG[2] = new PointF(position.X + GetX(8.3F, width), position.Y + GetY(6.5F, height));
            segmentG[3] = new PointF(position.X + GetX(9F, width), position.Y + GetY(7F, height));
            segmentG[4] = new PointF(position.X + GetX(8.2F, width), position.Y + GetY(7.5F, height));
            segmentG[5] = new PointF(position.X + GetX(2.9F, width), position.Y + GetY(7.5F, height));

            //Segment DP
            #endregion

            #region Draw Segments Outline
            g.FillPolygon(outline.Brush, segmentA);
            g.FillPolygon(outline.Brush, segmentB);
            g.FillPolygon(outline.Brush, segmentC);
            g.FillPolygon(outline.Brush, segmentD);
            g.FillPolygon(outline.Brush, segmentE);
            g.FillPolygon(outline.Brush, segmentF);
            g.FillPolygon(outline.Brush, segmentG);
            #endregion

            #region Fill Segments
            //Fill SegmentA
            if (IsNumberAvailable(number, 0, 2, 3, 5, 6, 7, 8, 9))
            {
                g.FillPolygon(fillPen.Brush, segmentA);
            }

            //Fill SegmentB
            if (IsNumberAvailable(number, 0, 1, 2, 3, 4, 7, 8, 9))
            {
                g.FillPolygon(fillPen.Brush, segmentB);
            }

            //Fill SegmentC
            if (IsNumberAvailable(number, 0, 1, 3, 4, 5, 6, 7, 8, 9))
            {
                g.FillPolygon(fillPen.Brush, segmentC);
            }

            //Fill SegmentD
            if (IsNumberAvailable(number, 0, 2, 3, 5, 6, 8, 9))
            {
                g.FillPolygon(fillPen.Brush, segmentD);
            }

            //Fill SegmentE
            if (IsNumberAvailable(number, 0, 2, 6, 8))
            {
                g.FillPolygon(fillPen.Brush, segmentE);
            }

            //Fill SegmentF
            if (IsNumberAvailable(number, 0, 4, 5, 6, 7, 8, 9))
            {
                g.FillPolygon(fillPen.Brush, segmentF);
            }

            //Fill SegmentG
            if (IsNumberAvailable(number, 2, 3, 4, 5, 6, 8, 9, -1))
            {
                g.FillPolygon(fillPen.Brush, segmentG);
            }
            #endregion
            
            //Draw decimal point
            if (dp)
            {
                g.FillEllipse(fillPen.Brush, new RectangleF(
                    position.X + GetX(10F, width), 
                    position.Y + GetY(12F, height),
                    width/7, 
                    width/7));
            }
        }

        /// <summary>
        /// Gets Relative X for the given width to draw digit
        /// </summary>
        /// <param name="x"></param>
        /// <param name="width"></param>
        /// <returns></returns>
        private float GetX(float x, float width)
        {
            return x * width / 12;
        }

        /// <summary>
        /// Gets relative Y for the given height to draw digit
        /// </summary>
        /// <param name="y"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        private float GetY(float y, float height)
        {
            return y * height / 15;
        }

        /// <summary>
        /// Returns true if a given number is available in the given list.
        /// </summary>
        /// <param name="number"></param>
        /// <param name="listOfNumbers"></param>
        /// <returns></returns>
        private bool IsNumberAvailable(int number, params int[] listOfNumbers)
        {
            if (listOfNumbers.Length > 0)
            {
                foreach (int i in listOfNumbers)
                {
                    if (i == number)
                        return true;
                }
            }
            return false;
        }
        
        /// <summary>
        /// Restricts the size to make sure the height and width are always same.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AquaGauge_Resize(object sender, EventArgs e)
        {
            if (this.Width < 136)
            {
                this.Width = 136;
            }
            if (oldWidth != this.Width)
            {
                this.Height = this.Width;
                oldHeight = this.Width;
            }
            if (oldHeight != this.Height)
            {
                this.Width = this.Height;
                oldWidth = this.Width;
            }
            
            // edited on 29-12-2010 by Bernardo Giovanni (http://www.settorezero.com)
            // added following two lines in order to resize entire control
            requiresRedraw = true;
            this.Invalidate();
        }
        #endregion
    }
}
