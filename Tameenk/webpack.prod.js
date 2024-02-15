const merge = require('webpack-merge');
const common = require('./webpack.common.js');
const CopyWebpackPlugin = require('copy-webpack-plugin');
const { AngularCompilerPlugin } = require('@ngtools/webpack');
const CleanWebpackPlugin = require('clean-webpack-plugin');
const path = require('path');

module.exports = merge(common, {
  mode: 'production',
  plugins: [
	new CleanWebpackPlugin(['dist']),
    new CopyWebpackPlugin([{
        from: 'assets',
        to: 'assets'
	}]),
	new AngularCompilerPlugin({
        tsConfigPath: './tsconfig.json',
        entryModule: './src/app/app.module#AppModule',
        sourceMap: true
    })
  ],
  output: {
    filename: '[name].[hash].js',
    path: path.resolve(__dirname, 'dist')
}
});