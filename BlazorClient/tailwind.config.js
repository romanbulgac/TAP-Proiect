module.exports = {
  content: [
    './**/*.html',
    './**/*.razor',
    './Pages/**/*.razor',
    './Shared/**/*.razor',
  ],
  darkMode: 'class',
  theme: {
    extend: {
      fontFamily: {
        sans: ['Inter var', 'Inter', 'Segoe UI', 'system-ui', 'sans-serif'],
        display: ['Inter var', 'Inter', 'Segoe UI', 'system-ui', 'sans-serif'],
      },
      animation: {
        'fade-in': 'fadeIn 0.3s ease-in-out',
        'fade-out': 'fadeOut 0.3s ease-in-out',
        'slide-in': 'slideIn 0.3s ease-in-out',
        'slide-out': 'slideOut 0.3s ease-in-out',
        'spin': 'spin 1s linear infinite',
        'pulse': 'pulse 2s cubic-bezier(0.4, 0, 0.6, 1) infinite',
        'bounce': 'bounce 1s infinite',
      },
      keyframes: {
        fadeIn: {
          '0%': { opacity: '0' },
          '100%': { opacity: '1' },
        },
        fadeOut: {
          '0%': { opacity: '1' },
          '100%': { opacity: '0' },
        },
        slideIn: {
          '0%': { transform: 'translateY(-10px)', opacity: '0' },
          '100%': { transform: 'translateY(0)', opacity: '1' },
        },
        slideOut: {
          '0%': { transform: 'translateY(0)', opacity: '1' },
          '100%': { transform: 'translateY(10px)', opacity: '0' },
        },
        spin: {
          '0%': { transform: 'rotate(0deg)' },
          '100%': { transform: 'rotate(360deg)' },
        },
        pulse: {
          '0%, 100%': { opacity: '1' },
          '50%': { opacity: '.5' },
        },
        bounce: {
          '0%, 100%': {
            transform: 'translateY(-25%)',
            animationTimingFunction: 'cubic-bezier(0.8, 0, 1, 1)',
          },
          '50%': {
            transform: 'translateY(0)',
            animationTimingFunction: 'cubic-bezier(0, 0, 0.2, 1)',
          },
        },
      },
      transitionProperty: {
        'width': 'width',
        'height': 'height',
        'spacing': 'margin, padding',
      },
      colors: {
        notion: {
          DEFAULT: '#2F3437',
          background: {
            light: '#ffffff',
            dark: '#191919',
          },
          sidebar: {
            light: '#fbfbfa',
            dark: '#202020',
          },
          text: {
            light: '#37352F',
            dark: '#E6E6E5',
            secondary: {
              light: '#6B6B6B',
              dark: '#999999',
            },
          },
          border: {
            light: '#EBEBEA',
            dark: '#2F3437',
          },
          hover: {
            light: '#F1F1F0',
            dark: '#2F3437',
          },
        },
      },
      boxShadow: {
        'soft': '0 1px 2px rgba(0, 0, 0, 0.05), 0 1px 3px rgba(0, 0, 0, 0.05)',
        'soft-lg': '0 2px 4px rgba(0, 0, 0, 0.06), 0 4px 6px rgba(0, 0, 0, 0.1)',
        'notion': '0 1px 3px rgba(0, 0, 0, 0.12), 0 1px 2px rgba(0, 0, 0, 0.24)',
        'notion-hover': '0 3px 6px rgba(0, 0, 0, 0.15), 0 2px 4px rgba(0, 0, 0, 0.12)',
      },
      borderRadius: {
        'xl': '0.75rem',
        '2xl': '1rem',
      },
      spacing: {
        '18': '4.5rem',
        '72': '18rem',
        '84': '21rem',
        '96': '24rem',
      },
      opacity: {
        '85': '0.85',
        '95': '0.95',
      },
      transitionProperty: {
        'width': 'width',
        'spacing': 'margin, padding',
      },
      keyframes: {
        fadeIn: {
          '0%': { opacity: '0' },
          '100%': { opacity: '1' },
        },
        slideInRight: {
          '0%': { transform: 'translateX(100%)' },
          '100%': { transform: 'translateX(0)' },
        },
      },
      animation: {
        fadeIn: 'fadeIn 0.2s ease-in-out',
        slideInRight: 'slideInRight 0.3s ease-out',
      },
    },
  },
  plugins: [],
}
