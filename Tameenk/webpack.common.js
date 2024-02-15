const HtmlWebpackPlugin = require('html-webpack-plugin');
const ScriptExtPlugin = require('script-ext-html-webpack-plugin');

module.exports = {
    entry: {
        main: './src/main.ts',
    },
  plugins: [
    new HtmlWebpackPlugin({
        filename: '../Views/Home/Index.cshtml',
        template: './Views/Home/Index.ng.cshtml',
        inject: true,

    }),
    new ScriptExtPlugin({
        defaultAttribute: 'defer'
    })
  ],
  resolve: {
    extensions: ['.ts', '.js']
    },
    module: {
        rules: [{
                test: /\.ts$/,
                loaders: ['@ngtools/webpack']
            },
            {
                test: /\.css$/,
                loader: 'raw-loader'
            },
            {
                test: /\.html$/,
                loader: 'raw-loader'
            }
        ]
    }
};