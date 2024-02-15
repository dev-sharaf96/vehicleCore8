const HtmlWebpackPlugin = require('html-webpack-plugin');
const CopyWebpackPlugin = require('copy-webpack-plugin');
const ScriptExtPlugin = require('script-ext-html-webpack-plugin');
const {
	AngularCompilerPlugin
} = require('@ngtools/webpack');

module.exports = function () {
	return {
		mode: 'production',
        entry: {
            main: './src/main.ts'
            },
		output: {
			path: __dirname + '/dist',
			filename: '[name].js',
			publicPath: '/'
		},
		resolve: {
			extensions: ['.ts', '.js']
		},
		devServer: {
			historyApiFallback: true,
			port: 9000
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
        },
        devtool: 'source-map',
		plugins: [
			new CopyWebpackPlugin([{
				from: 'assets',
				to: 'assets'
			}]),
			new HtmlWebpackPlugin({
				template: __dirname + '/src/index.html',
				output: __dirname + '/dist',
				inject: 'head'
			}),

			new ScriptExtPlugin({
				defaultAttribute: 'defer'
			}),
			new AngularCompilerPlugin({
				tsConfigPath: './tsconfig.json',
                entryModule: './src/app/app.module#AppModule',
				sourceMap: true
			})

		]
	};
}